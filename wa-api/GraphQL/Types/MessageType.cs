using wa_api.Data;
using wa_api.Models;

namespace wa_api.GraphQL.Types
{
	public class MessageType : ObjectType<Message>
	{
		protected override void Configure(IObjectTypeDescriptor<Message> descriptor)
		{
			base.Configure(descriptor);

			descriptor.Description("Represents any sent message");
		}
	}
}
