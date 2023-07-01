using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using wa_api.Controllers.Types;
using wa_api.Data;

namespace wa_api.Controllers
{
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

		[HttpPost]
		public async Task<ActionResult<AccountTakenPayload>> Post([FromBody] AccountTakenInput input)
		{
			var isUsernameValid = input.Username is not null
				&& input.Username.Length >= Constants.USERNAME_LENGTH_RANGE.from
				&& input.Username.Length <= Constants.USERNAME_LENGTH_RANGE.to;
			var isEmailValid = input.Email is not null
				&& input.Email.Length >=  Constants.EMAIL_LENGTH_RANGE.from
				&& input.Email.Length <= Constants.EMAIL_LENGTH_RANGE.to;

			await using var dbContext = _dbContextFactory.CreateDbContext();
			if (dbContext is null)
			{
				// TODO: Maybe throw some other exception?
				throw new NullReferenceException("Could not connect to database.");
			}

			var userUsername = isUsernameValid ? await dbContext.Users.FirstOrDefaultAsync(u => u.Username == input.Username) : null;
			var userEmail = isEmailValid ? await dbContext.Users.FirstOrDefaultAsync(u => u.Email == input.Email) : null;
			return new AccountTakenPayload(userUsername is not null, userEmail is not null);
		}
	}
}
