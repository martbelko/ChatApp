using wa_api.Data;
using wa_api.Models;

namespace wa_api.GraphQL
{
    public class Query
    {
        [UseDbContext(typeof(WaDbContext))]
        [UseFiltering]
        [UseSorting]
        public IQueryable<User> GetUser([ScopedService] WaDbContext context)
        {
            return context.Users;
        }

        [UseDbContext(typeof(WaDbContext))]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Message> GetMessage([ScopedService] WaDbContext context)
        {
            return context.Messages;
        }

        [UseDbContext(typeof(WaDbContext))]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Conversation> GetConversation([ScopedService] WaDbContext context)
        {
            return context.Conversations;
        }
    }
}
