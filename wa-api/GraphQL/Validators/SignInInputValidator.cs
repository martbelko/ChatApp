using FluentValidation;
using HotChocolate.Utilities;
using wa_api.Data;
using wa_api.GraphQL.Types;
using wa_api.Security;

namespace wa_api.GraphQL.Validators
{
	public class SignInInputValidator : AbstractValidator<SignInInput>
	{
		public SignInInputValidator(IServiceProvider provider)
		{
			provider.TryGetOrCreateService(typeof(WaDbContext), out WaDbContext context);
			RuleFor(p => p)
				.MustAsync((p, cancellationToken) => SecurityUtils.Authenticate(p, context, cancellationToken))
				.WithMessage("Invalid email or password");
		}
	}
}
