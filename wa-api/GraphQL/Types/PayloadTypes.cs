using wa_api.Models;

namespace wa_api.GraphQL.Types
{
    public record AddUserPayload(User? User, string? Error = null);
    public record AddMessagePayload(Message? Message, string? Error = null);
    public record AddConversationPayload(Conversation? Conversation, string? Error = null);
}
