using FluentValidation;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using HotChocolate.Utilities;
using wa_api.Data;
using wa_api.GraphQL.Types;

namespace wa_api.GraphQL.Middlewares.Validate
{
	internal class EmptyValidationMiddleware : IMiddleware
	{
		public override async Task Invoke(IMiddlewareContext context)
		{
			await Task.CompletedTask;
		}
	}

	public class ValidationMiddleware : IMiddleware
	{
		private readonly FieldDelegate _next;

		public ValidationMiddleware(FieldDelegate next)
		{
			_next = next;
		}

		public override async Task Invoke(IMiddlewareContext context)
		{
			context.Services.TryGetOrCreateService(typeof(WaDbContext), out WaDbContext s);

			foreach (var arg in context.Selection.Field.Arguments)
			{
				var type = arg.ContextData[ValidationConstants.Validators] as Type;
				if (type != null)
				{
					var validator = Activator.CreateInstance(type, context.Services) as dynamic;
					if (validator != null)
					{
						var val = context.ArgumentValue<object?>(arg.Name) as dynamic;
						await validator.ValidateAsync(val);
					}
				}
			}

			await _next(context);
		}
	}
}
