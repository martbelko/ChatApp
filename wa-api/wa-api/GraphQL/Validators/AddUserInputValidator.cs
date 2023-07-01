using FluentValidation;
using wa_api.GraphQL.Types;

namespace wa_api.GraphQL.Validators
{
	public class AddUserInputValidator : AbstractValidator<RegisterUserInput>
	{
		public AddUserInputValidator(IServiceProvider _)
		{
			RuleFor(p => p.Username)
				.NotNull()
				.WithMessage("Username must be provided.")
				.DependentRules(() =>
				{
					RuleFor(p => p.Username)
						.Length(Constants.USERNAME_LENGTH_RANGE.from, Constants.USERNAME_LENGTH_RANGE.to)
						.WithMessage(p => $"Username must be between {Constants.USERNAME_LENGTH_RANGE.from}" +
							$" and {Constants.USERNAME_LENGTH_RANGE.to} characters. " +
							$"You entered {p.Username.Length} characters.");
				});

			RuleFor(p => p.Password)
				.NotNull()
				.WithMessage("Password must be provided.")
				.DependentRules(() =>
				{
					RuleFor(p => p.Password)
						.Length(Constants.PASSWORD_LENGTH_RANGE.from, Constants.PASSWORD_LENGTH_RANGE.to)
						.WithMessage(p => $"Password must be between {Constants.PASSWORD_LENGTH_RANGE.from}" +
							$" and {Constants.PASSWORD_LENGTH_RANGE.to} characters. " +
							$"You entered {p.Password.Length} characters.");
				});

		}
	}
}
