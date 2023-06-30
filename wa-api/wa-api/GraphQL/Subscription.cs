using wa_api.Models;

namespace wa_api.GraphQL
{
	public class Subscription
	{
		[Subscribe]
		[Topic]
		public Message OnMessageAdded([EventMessage] Message message, [Service] IHttpContextAccessor httpContextAccessor)
		{
			if (httpContextAccessor.HttpContext is null)
			{
				return default!;
			}

			string? username;
			try
			{
				username = httpContextAccessor.HttpContext.Items["username"]!.ToString();
			}
			catch (Exception)
			{
				httpContextAccessor.HttpContext.Abort();
				return default!;
			}

			if (username is null)
			{
				httpContextAccessor.HttpContext.Abort();
				return default!;
			}

			// Check if message is from conversation where this user belongs
			if (message.Conversation.Members.Any(m => m.Username == username))
			{
				return message;
			}

			httpContextAccessor.HttpContext.Abort();
			return default!;
		}
	}
}
