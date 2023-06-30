using HotChocolate.Execution.Configuration;

namespace wa_api.GraphQL.Middlewares.Validate
{
	public static class IRequestExecutorBuilderExtensions
	{
		public static IRequestExecutorBuilder UseValidation(this IRequestExecutorBuilder builder)
		{
			return builder.UseField((provider, next) =>
			{
				return new ValidationMiddleware(next);
			});
		}
	}
}
