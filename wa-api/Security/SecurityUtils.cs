using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using wa_api.Data;
using wa_api.GraphQL.Types;

namespace wa_api.Security
{
	public static class SecurityUtils
	{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		private static WebApplicationBuilder Builder;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

		public static int REFRESH_TOKEN_DAYS { get; private set; }
		public static int ACCESS_TOKEN_MINUTES { get; private set; }
		public static readonly string REDIS_INSTANCE_NAME = "WhatsAppClone_";
		public static readonly string TOKEN_FAMILY_IDENTIFIER = "family";

		public static void Init(WebApplicationBuilder builder)
		{
			Builder = builder;
			REFRESH_TOKEN_DAYS = int.Parse(Builder.Configuration["Token:RefreshTokenDays"]!);
			ACCESS_TOKEN_MINUTES = int.Parse(Builder.Configuration["Token:AccessTokenMinutes"]!);
		}

		public static (byte[] Password, byte[] Salt) GeneratePassword(string password)
		{
			var rnd = new Random();
			var salt = new byte[512];
			rnd.NextBytes(salt);
			return (Password: GeneratePassword(password, salt), Salt: salt);
		}

		public static byte[] GeneratePassword(string password, byte[] salt)
		{
			return KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA512, 1000, 512);
		}

		public static string GenerateRefreshToken(string username, DateTime family)
		{
			return GenerateToken(username, DateTime.UtcNow.AddDays(REFRESH_TOKEN_DAYS), family, Builder.Configuration["Jwt:RefreshTokenKey"]!);
		}

		public static async Task<TokenValidationResult> ValidateRefreshToken(string refreshTokenString)
		{
			var handler = new JwtSecurityTokenHandler();
			return await handler.ValidateTokenAsync(refreshTokenString, GenerateRefreshTokenValidationParams());
		}

		public static string GenerateAccessToken(string username)
		{
			return GenerateToken(username, DateTime.UtcNow.AddMinutes(ACCESS_TOKEN_MINUTES), DateTime.Now, Builder.Configuration["Jwt:AccessTokenKey"]!);
		}

		[UseDbContext(typeof(WaDbContext))]
		public static async Task<bool> Authenticate(SignInInput input, [ScopedService] WaDbContext context, CancellationToken cancellationToken)
		{
			var user = await context.Users.FirstOrDefaultAsync(x => x.Email == input.Email, cancellationToken);
			if (user == null)
			{
				return false;
			}

			var pass = await context.Passwords.FirstOrDefaultAsync(x => x.UserId == user.Id);
			if (pass == null)
			{
				return false;
			}

			var hash = GeneratePassword(input.Password, pass.Salt);
			if (hash.Length != pass.Hash.Length)
			{
				return false;
			}

			for (int i = 0; i < hash.Length; ++i)
			{
				if (hash[i] != pass.Hash[i])
				{
					return false;
				}
			}

			return true;
		}

		public static TokenValidationParameters GenerateRefreshTokenValidationParams()
		{
			return GenerateTokenValidationParams(Builder.Configuration["Jwt:RefreshTokenKey"]!);
		}

		public static TokenValidationParameters GenerateAccessTokenValidationParams()
		{
			return GenerateTokenValidationParams(Builder.Configuration["Jwt:AccessTokenKey"]!);
		}

		private static TokenValidationParameters GenerateTokenValidationParams(string signKey)
		{
			return new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				LifetimeValidator = (notBefore, expires, token, param) =>
				{
					return expires.HasValue && expires! >= DateTime.UtcNow;
				},
				ValidateIssuerSigningKey = true,
				ValidIssuer = Builder.Configuration["Jwt:Issuer"]!,
				ValidAudience = Builder.Configuration["Jwt:Audience"]!,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey))
			};
		}

		private static string GenerateToken(string username, DateTime expires, DateTime tokenFamily, string signKey)
		{
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
			var claims = new[]
			{
				new Claim(ClaimTypes.NameIdentifier, username),
				new Claim(TOKEN_FAMILY_IDENTIFIER, tokenFamily.ToString())
			};

			var token = new JwtSecurityToken(
				issuer: Builder.Configuration["Jwt:Issuer"]!,
				audience: Builder.Configuration["Jwt:Audience"]!,
				claims: claims,
				expires: expires,
				signingCredentials: credentials
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
