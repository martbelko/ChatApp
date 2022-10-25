namespace wa_api.GraphQL.Types
{
	public record AddUserInput(string Username);
	public record AddMessageInput(int UserId, int ConversationId, string Content);
	public record AddConversationInput(List<int> UsersIds);
}
