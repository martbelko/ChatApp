using System.Reflection;
using wa_api.Data;
using wa_api.Models;

namespace wa_api.GraphQL.Types.Users
{
	public class GenericUserType : ObjectType<GenericUser>
	{
		protected override void Configure(IObjectTypeDescriptor<GenericUser> descriptor)
		{
			base.Configure(descriptor);

			descriptor.Description("Represents any user who has been registered");

			// Block the password field from returning
			var personalProps = typeof(User)
				.GetProperties()
				.Where(x => x.GetCustomAttributes(typeof(Personal)).Count() > 0)
				.ToArray();

			// Block all the other personal fields from returning
			foreach (var prop in personalProps)
			{
				descriptor
					.Field(prop)
					.Ignore();
			}
		}
	}

	public class UserType : ObjectType<User>
	{
		protected override void Configure(IObjectTypeDescriptor<User> descriptor)
		{
			base.Configure(descriptor);

			descriptor.Description("Represents any user who has been registered");

			// Block the password field from returning
			descriptor
				.Field(p => p.Password)
				.Ignore();

			// Rest of the resolvers
			descriptor
				.Field(p => p.Messages)
				.Cost(3)
				.ResolveWith<Resolvers>(p => p.GetMessages(default!, default!))
				.UseDbContext<WaDbContext>()
				.Description("List of messages sent by the user");

			descriptor
				.Field(p => p.Conversations)
				.Cost(2)
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
