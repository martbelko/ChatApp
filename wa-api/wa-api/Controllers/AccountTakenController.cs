using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using wa_api.Controllers.Types;
using wa_api.Data;

namespace wa_api.Controllers
{
	public class CustomAttrAttribute : ValidationAttribute
	{
		protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
		{
			if (value is null)
			{
				throw new ValidationException();
			}

			var input = value as AccountTakenInput;
			if (input is null)
			{
				throw new ValidationException();
			}

			return ValidationResult.Success;
		}
	}

	public record AccountTakenPayload(bool UsernameTaken, bool EmailTaken);

	[ApiController]
	[Route("[controller]")]
	public class AccountTakenController : ControllerBase
	{
		private readonly IDbContextFactory<WaDbContext> _dbContextFactory;

		public AccountTakenController(IDbContextFactory<WaDbContext> dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		// TODO: Validate input before even accessing database
		[HttpPost]
		public async Task<ActionResult<AccountTakenPayload>> Post([FromBody][CustomAttr] AccountTakenInput input)
		{
			await using var dbContext = _dbContextFactory.CreateDbContext();
			if (dbContext is null)
			{
				// TODO: Maybe throw some other exception?
				throw new NullReferenceException("Could not connect to database.");
			}

			var userUsername = input.Username is null ? null : await dbContext.Users.FirstOrDefaultAsync(u => u.Username == input.Username);
			var userEmail = input.Email is null ? null : await dbContext.Users.FirstOrDefaultAsync(u => u.Email == input.Email);
			return new AccountTakenPayload(userUsername is not null, userEmail is not null);
		}
	}
}
