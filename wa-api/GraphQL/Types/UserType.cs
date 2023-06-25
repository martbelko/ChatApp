using wa_api.Data;
using wa_api.Models;

namespace wa_api.GraphQL.Types.Users
{
	public class GenericUserType : GenericGraphQLType<GenericUser>
	{
		protected override void Configure(IObjectTypeDescriptor<GenericUser> descriptor)
		{
			base.Configure(descriptor);

			descriptor.Description("Represents any user who has been registered");
		}
	}

	public class UserType : GenericGraphQLType<User>
	{
		protected override void Configure(IObjectTypeDescriptor<User> descriptor)
		{
			base.Configure(descriptor);

			descriptor.Description("Represents any user who has been registered");

			descriptor
				.Field(p => p.Messages)
				.Cost(3) // TODO: Maybe change?
				.ResolveWith<Resolvers>(p => p.GetMessages(default!, default!))
				.UseDbContext<WaDbContext>()
				.Description("List of messages sent by the user");

			descriptor
				.Field(p => p.Conversations)
				.Cost(2) // TODO: Maybe change?
				.ResolveWith<Resolvers>(p => p.GetConversations(default!, default!))
				.UseDbContext<WaDbContext>()
				.Description("List of active conversations for the user");
		}

		private class Resolvers
		{
			public IQueryable<Message> GetMessages([Parent] User user, [ScopedService] WaDbContext context)
			{
				return context.Messages.Where(m => m.Author.Id == user.Id);
			}

			public IQueryable<Conversation> GetConversations([Parent] User user, [ScopedService] WaDbContext context)
			{
				return context.Conversations.Where(p => p.Members.Any(m => m.Id == user.Id));
			}
		}
	}
}
