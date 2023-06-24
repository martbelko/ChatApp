using Microsoft.EntityFrameworkCore;
using wa_api.Data;
using wa_api.Models;

namespace wa_api.GraphQL.Types.Messages
{
    public class MessageType : ObjectType<Message>
    {
        protected override void Configure(IObjectTypeDescriptor<Message> descriptor)
        {
            base.Configure(descriptor);

            descriptor.Description("Represents any sent message");

            descriptor
                .Field(p => p.Author)
                .ResolveWith<Resolvers>(p => p.GetAuthor(default!, default!))
                .UseDbContext<WaDbContext>()
                .Description("Author of the message");

            descriptor
                .Field(p => p.Conversation)
                .ResolveWith<Resolvers>(p => p.GetConversation(default!, default!))
                .UseDbContext<WaDbContext>()
                .Description("Conversation that the message belongs to");
        }

        private class Resolvers
        {
            public async Task<User> GetAuthor([Parent] Message message, [ScopedService] WaDbContext context)
            {
                return await context.Users.FirstAsync(u => u.Messages.Any(m => m.Id == message.Id));
            }

            public async Task<Conversation> GetConversation([Parent] Message message, [ScopedService] WaDbContext context)
            {
                return await context.Conversations.FirstAsync(c => c.Messages.Any(m => m.Id == message.Id));
            }
        }
    }
}
