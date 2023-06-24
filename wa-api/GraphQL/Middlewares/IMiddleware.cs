using HotChocolate.Resolvers;

namespace wa_api.GraphQL.Middlewares
{
	public abstract class IMiddleware
	{
		public virtual async Task Invoke(IMiddlewareContext context)
		{
			await Task.CompletedTask;
		}
	}
}
