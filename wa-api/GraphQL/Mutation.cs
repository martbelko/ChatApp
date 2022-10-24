using EntityFramework.Exceptions.Common;
using HotChocolate.Subscriptions;
using Microsoft.EntityFrameworkCore;
using wa_api.Data;
using wa_api.GraphQL.Types;
using wa_api.Models;

namespace wa_api.GraphQL
{
    public class Mutation
    {
        [UseDbContext(typeof(WaDbContext))]
        public async Task<AddUserPayload> AddUserAsync(AddUserInput input, [ScopedService] WaDbContext context)
        {
            var user = new User
            {
                Username = input.Username
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

            return new AddUserPayload(user);
        }

        [UseDbContext(typeof(WaDbContext))]
        public async Task<AddMessagePayload> AddMessageAsync(
            AddMessageInput input,
            [ScopedService] WaDbContext context,
            [Service] ITopicEventSender eventSender,
            CancellationToken cancellationToken)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == input.UserId, cancellationToken);
            if (user == null)
            {
                return new AddMessagePayload(null, "Invalid author");
            }

            var conversation = await context.Conversations.FirstOrDefaultAsync(u => u.Id == input.ConversationId, cancellationToken);
            if (conversation == null)
            {
                return new AddMessagePayload(null, "Invalid conversation");
            }

            if (input.Content == null || input.Content.Length == 0)
            {
                return new AddMessagePayload(null, "Invalid message content");
            }

            var message = new Message
            {
                Author = user,
                Conversation = conversation,
                Content = input.Content,
                Time = DateTime.UtcNow
            };

            await context.Messages.AddAsync(message, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            await eventSender.SendAsync(nameof(Subscription.OnMessageAdded), message, cancellationToken);

            return new AddMessagePayload(message);
        }

        [UseDbContext(typeof(WaDbContext))]
        public async Task<AddConversationPayload> AddConversationAsync(
            AddConversationInput input,
            [ScopedService] WaDbContext context,
            [Service] ITopicEventSender eventSender,
            CancellationToken cancellationToken)
        {
            if (input.UsersIds.Count < 2)
            {
                return new AddConversationPayload(null, "Input does not contain at least 2 users");
            }

            var users = await context.Users.Where(u => input.UsersIds.Contains(u.Id)).ToListAsync(cancellationToken);
            if (users.Count != input.UsersIds.Count)
            {
                return new AddConversationPayload(null, "Input contains invalid user ids");
            }

            var conversation = new Conversation
            {
                Members = users
            };

            await context.Conversations.AddAsync(conversation, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            await eventSender.SendAsync(nameof(Subscription.OnConversationAdded), conversation, cancellationToken);

            return new AddConversationPayload(conversation);
        }
    }
}
