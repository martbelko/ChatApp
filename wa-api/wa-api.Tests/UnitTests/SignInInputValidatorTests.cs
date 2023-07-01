using wa_api.GraphQL.Types;
using wa_api.GraphQL.Validators;

namespace UnitTests
{
	public class AddUserInputValidatorTests
	{
		[Theory]
		[InlineData("testusername", "testpassword")]
		[InlineData("sameusername", "samepassword")]
		[InlineData("IHaveFancyUsername01", "testpassword")]
		[InlineData("?_-IHaveEvenFancier*0_name01']", "IHaveFancyPass01")]
		[InlineData("?_-IHaveEvenFancier*0_name01']", "?_-IHaveEvenFancier*0_Pass1']!")]
		public async Task Validate_ShouldBeOk(string username, string password)
		{
			var validator = new AddUserInputValidator(null!);
			var actual = await validator.ValidateAsync(new RegisterUserInput(username, password));

			Assert.NotNull(actual);
			Assert.True(actual.IsValid, actual.Errors.Count > 0 ? actual.Errors[0].ToString() : null);
			Assert.Empty(actual.Errors);
		}

		[Fact]
		public async Task Validate_NullUsername()
		{
			var expectedError = "Username must be provided.";

			var validator = new AddUserInputValidator(null!);
			var actual = await validator.ValidateAsync(new RegisterUserInput(null, "testpassword"));

			Assert.NotNull(actual);
			Assert.False(actual.IsValid);
			Assert.NotEmpty(actual.Errors);
			Assert.True(actual.Errors.Count == 1);
			Assert.Equal(expectedError, actual.Errors[0].ErrorMessage);
		}

		[Theory]
		[InlineData("", "validpassword")]
		[InlineData("", "anotherpassword")]
		[InlineData("thisisaverylonglongusername010101", "againokaypassword")]
		[InlineData("thisusernamehasexacly31characte", "wellpassword")]
		[InlineData("not", "againokaypassword01")]

		public async Task Validate_InvalidUsername(string username, string password)
		{
			var len = username is null ? 0 : username.Length;
			var expectedError = $"Username must be between 4 and 30 characters. You entered {len} characters.";

			var validator = new AddUserInputValidator(null!);
			var actual = await validator.ValidateAsync(new RegisterUserInput(username, password));

			Assert.NotNull(actual);
			Assert.False(actual.IsValid);
			Assert.NotEmpty(actual.Errors);
			Assert.True(actual.Errors.Count == 1);
			Assert.Equal(expectedError, actual.Errors[0].ErrorMessage);
		}

		[Fact]
		public async Task Validate_NullPassword()
		{
			var expectedError = "Password must be provided.";

			var validator = new AddUserInputValidator(null!);
			var actual = await validator.ValidateAsync(new RegisterUserInput("testusername", null));

			Assert.NotNull(actual);
			Assert.False(actual.IsValid);
			Assert.NotEmpty(actual.Errors);
			Assert.True(actual.Errors.Count == 1);
			Assert.Equal(expectedError, actual.Errors[0].ErrorMessage);
		}

		[Theory]
		[InlineData("validusername", "")]
		[InlineData("anothervalidusername", "")]
		[InlineData("againokayusername", "shortpa")]
		[InlineData("validusernameagain", @"amwxOXjZXbvMxnZvpRpnwSUNeuM809ihi\r\nwMU1GKeigfzkkvdQ46miGrpIr6hWHs6a\r\n7cVvjr9DHuwJNVjCDX6iyIoO090xFOYx\r\nYJuVocX59B1tjhKK0UYsGU8n3amoXdXc")]

		public async Task Validate_InvalidPassword(string username, string password)
		{
			var expectedError = $"Password must be between 8 and 128 characters. You entered {password.Length} characters.";

			var validator = new AddUserInputValidator(null!);
			var actual = await validator.ValidateAsync(new RegisterUserInput(username, password));

			Assert.NotNull(actual);
			Assert.False(actual.IsValid);
			Assert.NotEmpty(actual.Errors);
			Assert.True(actual.Errors.Count == 1);
			Assert.Equal(expectedError, actual.Errors[0].ErrorMessage);
		}

		[Fact]
		public async Task Validate_NullUsernameNullPassword()
		{
			var expectedErrorUsername = "Username must be provided.";
			var expectedErrorPassword = "Password must be provided.";

			var validator = new AddUserInputValidator(null!);
			var actual = await validator.ValidateAsync(new RegisterUserInput(null, null));

			Assert.NotNull(actual);
			Assert.False(actual.IsValid);
			Assert.NotEmpty(actual.Errors);
			Assert.True(actual.Errors.Count == 2);
			Assert.Equal(expectedErrorUsername, actual.Errors[0].ErrorMessage);
			Assert.Equal(expectedErrorPassword, actual.Errors[1].ErrorMessage);
		}

		[Theory]
		[InlineData("", "")]
		[InlineData("thisisaverylonglongusername010101", "")]
		[InlineData("", "shortpa")]
		[InlineData("sho", @"amwxOXjZXbvMxnZvpRpnwSUNeuM809ihi\r\nwMU1GKeigfzkkvdQ46miGrpIr6hWHs6a\r\n7cVvjr9DHuwJNVjCDX6iyIoO090xFOYx\r\nYJuVocX59B1tjhKK0UYsGU8n3amoXdXc")]
		[InlineData("", "short")]

		public async Task Validate_InvalidUsernameInvalidPassword(string username, string password)
		{
			var expectedErrorUsername = $"Username must be between 4 and 30 characters. You entered {username.Length} characters.";
			var expectedErrorPassword = $"Password must be between 8 and 128 characters. You entered {password.Length} characters.";

			var validator = new AddUserInputValidator(null!);
			var actual = await validator.ValidateAsync(new RegisterUserInput(username, password));

			Assert.NotNull(actual);
			Assert.False(actual.IsValid);
			Assert.NotEmpty(actual.Errors);
			Assert.True(actual.Errors.Count == 2);
			Assert.Equal(expectedErrorUsername, actual.Errors[0].ErrorMessage);
			Assert.Equal(expectedErrorPassword, actual.Errors[1].ErrorMessage);
		}
	}
}
