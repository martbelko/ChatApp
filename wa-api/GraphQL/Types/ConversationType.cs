using wa_api.Data;
using wa_api.Models;

namespace wa_api.GraphQL.Types.Conversations
{
	public class ConversationType : ObjectType<Conversation>
	{
		protected override void Configure(IObjectTypeDescriptor<Conversation> descriptor)
		{
			base.Configure(descriptor);

			descriptor.Description("Represents any conversation");

			descriptor
				.Field(p => p.Messages)
				.ResolveWith<Resolvers>(p => p.GetMessages(default!, default!))
				.UseDbContext<WaDbContext>()
				.Description("List of messages sent in the conversation");

			descriptor
				.Field(p => p.Members)
				.ResolveWith<Resolvers>(p => p.GetMembers(default!, default!))
				.UseDbContext<WaDbContext>()
				.Description("List of members in the conversations");
		}

		private class Resolvers
		{
			public IQueryable<Message> GetMessages([Parent] Conversation conversation, [ScopedService] WaDbContext context)
			{
				return context.Messages.Where(m => m.Conversation.Id == conversation.Id);
			}

			public IQueryable<User> GetMembers([Parent] Conversation conversation, [ScopedService] WaDbContext context)
			{
				return context.Users.Where(p => p.Conversations.Any(c => c.Id == conversation.Id));
			}
		}
	}
}
