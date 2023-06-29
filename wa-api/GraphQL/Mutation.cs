using EntityFramework.Exceptions.Common;
using FluentValidation;
using HotChocolate.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using wa_api.Data;
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

		private static void RemoveExpiredTokens(WaDbContext dbContext)
		{
			var invalidTokenFilter = (RefreshToken t) =>
			{
				var handler = new JwtSecurityTokenHandler();
				var result = handler.ValidateTokenAsync(t.Content, SecurityUtils.GenerateRefreshTokenValidationParams()).Result;
				return result.IsValid;
			};

			var expiredTokens = dbContext.RefreshTokens.Where(invalidTokenFilter);
			dbContext.RemoveRange(expiredTokens);
		}

		[UseDbContext(typeof(WaDbContext))]
		public async Task<GetAccessTokenPayload> RefreshAccessTokenAsync(GetAccessTokenInput input, [ScopedService] WaDbContext dbContext)
		{
			var handler = new JwtSecurityTokenHandler();
			var result = await handler.ValidateTokenAsync(input.RefreshToken, SecurityUtils.GenerateRefreshTokenValidationParams());
			try
			{
				var token = handler.ReadJwtToken(input.RefreshToken).Claims.ToDictionary(p => p.Type, p => p.Value);
				var username = token[ClaimTypes.NameIdentifier];
				var familyStr = token[SecurityUtils.TOKEN_FAMILY_IDENTIFIER];
				var dbUser = await dbContext.Users.FirstAsync(u => u.Username == username);
				if (dbUser is null || username is null || DateTime.TryParse(familyStr, out var family))
				{
					return new GetAccessTokenPayload(null, null, "Refresh token is invalid");
				}

				RemoveExpiredTokens(dbContext);

				var dbRefreshToken = await dbContext.RefreshTokens.FirstAsync(t => t.Owner.Username == username && t.Content ==  input.RefreshToken);
				if (dbRefreshToken is null)
				{
					var familyRefreshToken = await dbContext.RefreshTokens.FirstAsync(t => t.Family == family!);
					if (familyRefreshToken is not null)
					{
						dbContext.RefreshTokens.Remove(familyRefreshToken);
					}

					await dbContext.SaveChangesAsync();
					return new GetAccessTokenPayload(null, null, "Breach detected!");
				}

				dbContext.RefreshTokens.Remove(dbRefreshToken);

				var newRefreshToken = SecurityUtils.GenerateRefreshToken(username, family);
				await dbContext.RefreshTokens.AddAsync(new RefreshToken
				{
					Content = newRefreshToken,
					Family = family,
					Owner = dbUser
				});

				var saveDbTask = dbContext.SaveChangesAsync();
				var newAccessToken = SecurityUtils.GenerateAccessToken(username);
				await saveDbTask;
				return new GetAccessTokenPayload(newAccessToken, newRefreshToken);
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

			RemoveExpiredTokens(context);

			var timeNow = DateTime.UtcNow;
			var refreshToken = SecurityUtils.GenerateRefreshToken(input.Email, timeNow);

			await context.RefreshTokens.AddAsync(new RefreshToken
			{
				Content = refreshToken,
				Family = timeNow,
				Owner = user
			});

			await context.SaveChangesAsync();
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
