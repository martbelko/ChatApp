using FluentValidation;
using FluentValidation.Results;
using HotChocolate.Resolvers;
using HotChocolate.Utilities;
using wa_api.Data;

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
				arg.ContextData.TryGetValue(ValidationConstants.Validators, out var obj);
				var type = obj is null ? null : obj as Type;
				if (type is not null)
				{
					var validator = Activator.CreateInstance(type, context.Services) as dynamic;
					if (validator is not null)
					{
						var val = context.ArgumentValue<object?>(arg.Name) as dynamic;
						if (val is null)
						{
							throw new NullReferenceException("Nothing to validate.");
						}

						var result = await validator.ValidateAsync(val) as ValidationResult;
						if (result is not null && !result.IsValid)
						{
							throw new ValidationException(result.Errors);
						}
					}
				}
			}

			await _next(context);
		}
	}
}
