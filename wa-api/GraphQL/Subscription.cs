using wa_api.Models;

namespace wa_api.GraphQL
{
	public class Subscription
	{
		[Subscribe]
		[Topic]
		public Message OnMessageAdded([EventMessage] Message message)
		{
			return message;
		}

		[Subscribe]
		[Topic]
		public Conversation OnConversationAdded([EventMessage] Conversation conversation)
		{
			return conversation;
		}
	}
}
