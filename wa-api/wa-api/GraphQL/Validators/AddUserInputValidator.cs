using FluentValidation;
using wa_api.GraphQL.Types;

namespace wa_api.GraphQL.Validators
{
	public class AddUserInputValidator : AbstractValidator<RegisterUserInput>
	{
		public AddUserInputValidator(IServiceProvider _)
		{
			// TODO: Add messages with error codes
			RuleFor(p => p.Username)
				.NotNull()
				.WithMessage("Username must be provided.")
				.DependentRules(() =>
				{
					RuleFor(p => p.Username)
						.Length(4, 30)
						.WithMessage(p => $"Username must be between 4 and 30 characters. You entered {p.Username.Length} characters.");
				});

			RuleFor(p => p.Password)
				.NotNull()
				.WithMessage("Password must be provided.")
				.DependentRules(() =>
				{
					RuleFor(p => p.Password)
						.Length(8, 128)
						.WithMessage(p => $"Password must be between 8 and 128 characters. You entered {p.Password.Length} characters.");
				});

		}
	}
}
