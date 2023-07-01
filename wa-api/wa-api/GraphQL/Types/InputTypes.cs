namespace wa_api.GraphQL.Types
{
	public record SignInInput(string Email, string Password);
	public record GetAccessTokenInput(string RefreshToken);
	public record RegisterUserInput(string Username, string Password);
	public record AddMessageInput(int ConversationId, string Content);
	public record AddConversationInput(List<int> UsersIds, string Message);
	public record OnMessageAddedInput(int UserId);
}
