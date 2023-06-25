using FluentValidation;
using wa_api.GraphQL.Types;

namespace wa_api.GraphQL.Validators
{
	class AddUserInputValidator : AbstractValidator<AddUserInput>
	{
		public AddUserInputValidator(IServiceProvider provider)
		{
			// TODO: Add messages with error codes
			RuleFor(p => p.Username).NotEmpty().Length(4, 30);
			RuleFor(p => p.Password).NotEmpty().Length(8, 30);
		}
	}
}
