using System.Reflection;

namespace wa_api.GraphQL.Types
{
	public abstract class GenericGraphQLType<T> : ObjectType<T>
	{
		protected override void Configure(IObjectTypeDescriptor<T> descriptor)
		{
			base.Configure(descriptor);

			var personalProps = typeof(T)
				.GetProperties()
				.Where(x => x.GetCustomAttributes(typeof(DontShareAttribute)).Count() > 0)
				.ToArray();

			foreach (var prop in personalProps)
			{
				descriptor
					.Field(prop)
					.Ignore();
			}
		}
	}
}
