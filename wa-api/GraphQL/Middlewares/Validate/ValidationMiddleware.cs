using FluentValidation.Results;
using HotChocolate.Resolvers;
using HotChocolate.Utilities;
using wa_api.Data;
using wa_api.Exceptions;

namespace wa_api.GraphQL.Middlewares.Validate
{
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
						var result = await validator.ValidateAsync(val) as ValidationResult;
						if (result is not null && !result.IsValid)
						{
							// TODO: Format error message as well with the error code
							throw new ValidationException(result.Errors[0].ErrorMessage);
						}
					}
				}
			}

			await _next(context);
		}
	}
}
