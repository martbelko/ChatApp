using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using wa_api.Data;
using wa_api.GraphQL.Middlewares.Validate;
using wa_api.Models;

namespace wa_api.GraphQL
{
	public class TestReturn
	{
		public string Content { get; set; } = default!;
	}

	public class Query
	{
		public TestReturn GetTest()
		{
			var r = new TestReturn
			{
				Content = "Hello, World!"
			};
			return r;
		}

		[UseDbContext(typeof(WaDbContext))]
		public async Task<User?> GetMeAsync([ScopedService] WaDbContext context, ClaimsPrincipal claims, CancellationToken cancellationToken)
		{
			var username = claims.FindFirstValue(ClaimTypes.NameIdentifier);
			if (username is null)
			{
				return null;
			}

			return await context.Users.FirstOrDefaultAsync(p => p.Username == username);
		}

		[UseDbContext(typeof(WaDbContext))]
		[UseFiltering]
		[UseSorting]
		public IQueryable<GenericUser>? GetUser([ScopedService] WaDbContext context, ClaimsPrincipal claims)
		{
			var username = claims.FindFirstValue(ClaimTypes.NameIdentifier);
			if (username is null)
			{
				return null;
			}

			return context.Users;
		}

		[UseDbContext(typeof(WaDbContext))]
		[UsePaging(MaxPageSize = 10)]
		[UseFiltering]
		[UseSorting]
		public IQueryable<Message>? GetMessage([ScopedService] WaDbContext context, ClaimsPrincipal claims)
		{
			var username = claims.FindFirstValue(ClaimTypes.NameIdentifier);
			if (username is null)
			{
				return null;
			}

			var conversations = context.Conversations.Include(c => c.Members).Where(c => c.Members.Any(m => m.Username == username)).Select(c => c.Id);
			return context.Messages.Include(m => m.Conversation).Where(m => conversations.Contains(m.Conversation.Id));
		}

		[UseDbContext(typeof(WaDbContext))]
		[UseFiltering]
		[UseSorting]
		public IQueryable<Conversation>? GetConversation([ScopedService] WaDbContext context, ClaimsPrincipal claims)
		{
			var username = claims.FindFirstValue(ClaimTypes.NameIdentifier);
			if (username is null)
			{
				return null;
			}

			return context.Conversations.Include(c => c.Members).Where(p => p.Members.Any(m => m.Username == username));
		}
	}
}
