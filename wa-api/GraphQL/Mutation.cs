using EntityFramework.Exceptions.Common;
using FluentValidation;
using HotChocolate.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using wa_api.Data;
using wa_api.Extensions;
using wa_api.GraphQL.Middlewares.Validate;
using wa_api.GraphQL.Types;
using wa_api.GraphQL.Validators;
using wa_api.Models;
using wa_api.Security;

namespace wa_api.GraphQL
{
	public class Mutation
	{
		private readonly IDistributedCache _cache;

		public Mutation(IDistributedCache cache)
		{
			_cache = cache;
		}

		public async Task<GetAccessTokenPayload> RefreshAccessTokenAsync(GetAccessTokenInput input, ClaimsPrincipal claims)
		{
			var handler = new JwtSecurityTokenHandler();
			var result = await handler.ValidateTokenAsync(input.RefreshToken, SecurityUtils.GenerateRefreshTokenValidationParams());
			try
			{
				var token = handler.ReadJwtToken(input.RefreshToken).Claims.ToDictionary(p => p.Type, p => p.Value);
				var username = token[ClaimTypes.NameIdentifier];
				var redisKey = $"{SecurityUtils.REDIS_INSTANCE_NAME}{username}";
				var allTokens = await _cache.GetRecordAsync<List<string>>(redisKey);

				if (allTokens is not null && allTokens.Contains(input.RefreshToken))
				{
					allTokens.Remove(input.RefreshToken);

					var accessToken = SecurityUtils.GenerateAccessToken(username);
					var refreshToken = SecurityUtils.GenerateRefreshToken(username);

					allTokens.Add(refreshToken);
					await _cache.SetRecordAsync(redisKey, allTokens, null, TimeSpan.FromDays(SecurityUtils.REFRESH_TOKEN_DAYS));

					return new GetAccessTokenPayload(accessToken, refreshToken, null);
				}

				await _cache.RemoveAsync(redisKey);
				return new GetAccessTokenPayload(null, null, "Refresh token is invalid");
			}
			catch (Exception)
			{
				return new GetAccessTokenPayload(null, null, "Refresh token is invalid");
			}
		}

		[UseDbContext(typeof(WaDbContext))]
		public async Task<SignInPayload> SignInAsync([UseValidate<SignInInputValidator>] SignInInput input, [ScopedService] WaDbContext context)
		{
			var user = await context.Users.FirstOrDefaultAsync((u) => u.Email == input.Email);
			if (user is null)
			{
				return new SignInPayload(null, "Invalid email or password");
			}

			var refreshToken = SecurityUtils.GenerateRefreshToken(input.Email);

			var allTokens = await _cache.GetRecordAsync<List<string>>($"{SecurityUtils.REDIS_INSTANCE_NAME}{input.Email}");
			if (allTokens is null)
			{
				allTokens = new List<string>();
			}

			allTokens.Add(refreshToken);
			await _cache.SetRecordAsync($"{SecurityUtils.REDIS_INSTANCE_NAME}{user.Username}",
				allTokens, null, TimeSpan.FromDays(SecurityUtils.REFRESH_TOKEN_DAYS));

			user.RefreshTokens.Add(new RefreshToken
			{
				Content = refreshToken
			});

			return new SignInPayload(refreshToken);
		}

		[UseDbContext(typeof(WaDbContext))]
		public async Task<AddUserPayload> AddUserAsync([UseValidate<AddUserInputValidator>] AddUserInput input, [ScopedService] WaDbContext context)
		{
			var (hash, salt) = SecurityUtils.GeneratePassword(input.Password);

			var user = new User
			{
				Username = input.Username,
				Password = new Password
				{
					Salt = salt,
					Hash = hash
				}
			};

			await context.Users.AddAsync(user);
			try
			{
				await context.SaveChangesAsync();
			}
			catch (UniqueConstraintException)
			{
				return new AddUserPayload(null, "Username already taken");
			}
			catch (Exception)
			{
				return new AddUserPayload(null, "Unknown error");
			}

			return new AddUserPayload(user);
		}

		[UseDbContext(typeof(WaDbContext))]
		public async Task<AddMessagePayload> AddMessageAsync(
			AddMessageInput input,
			[ScopedService] WaDbContext context,
			[Service] ITopicEventSender eventSender,
			CancellationToken cancellationToken,
			ClaimsPrincipal claims)
		{
			var username = claims.FindFirstValue(ClaimTypes.NameIdentifier);
			if (username is null)
			{
				return new AddMessagePayload(null, "Invalid access token");
			}

			var user = await context.Users.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
			if (user is null)
			{
				return new AddMessagePayload(null, "Invalid author");
			}

			var conversation = await context.Conversations.FirstOrDefaultAsync(u => u.Id == input.ConversationId, cancellationToken);
			if (conversation is null)
			{
				return new AddMessagePayload(null, "Invalid conversation");
			}

			if (input.Content is null || input.Content.Length == 0)
			{
				return new AddMessagePayload(null, "Invalid message content");
			}

			var message = new Message
			{
				Author = user,
				Conversation = conversation,
				Content = input.Content,
				CreatedAt = DateTime.UtcNow
			};

			await context.Messages.AddAsync(message, cancellationToken);
			await context.SaveChangesAsync(cancellationToken);

			var topic = $"{conversation.Id}_{nameof(Subscription.OnMessageAdded)}";
			await eventSender.SendAsync(topic, message, cancellationToken);

			return new AddMessagePayload(message);
		}

		[UseDbContext(typeof(WaDbContext))]
		public async Task<AddConversationPayload> AddConversationAsync(
			AddConversationInput input,
			[ScopedService] WaDbContext context,
			[Service] ITopicEventSender eventSender,
			CancellationToken cancellationToken,
			ClaimsPrincipal claims)
		{
			var username = claims.FindFirstValue(ClaimTypes.NameIdentifier);
			if (username == null)
			{
				return new AddConversationPayload(null, "Invalid access token");
			}

			if (input.UsersIds.Count == 0)
			{
				return new AddConversationPayload(null, "Input does not contain at least 1 user");
			}

			var members = await context.Users.Where(u => input.UsersIds.Contains(u.Id)).ToListAsync(cancellationToken);
			if (members.Count != input.UsersIds.Count)
			{
				return new AddConversationPayload(null, "Input contains invalid user ids");
			}

			// Don't forget to add sender
			var user = context.Users.FirstOrDefault(u => u.Username == username);
			if (user == null)
			{
				return new AddConversationPayload(null, "Invalid user");
			}

			members.Add(user);

			var conversation = new Conversation
			{
				Members = members
			};

			var message = new Message
			{
				Author = user,
				Content = input.Message,
				Conversation = conversation,
				CreatedAt = DateTime.UtcNow
			};

			conversation.Messages.Add(message);

			await context.Conversations.AddAsync(conversation, cancellationToken);
			await context.SaveChangesAsync(cancellationToken);

			var topic = $"{nameof(Subscription.OnMessageAdded)}";
			await eventSender.SendAsync(topic, message, cancellationToken);

			return new AddConversationPayload(conversation);
		}
	}
}
