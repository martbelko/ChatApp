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
	public class SecurityUtils
	{
		private readonly string _refreshTokenSignKey;
		private readonly string _accessTokenSignKey;
		private readonly string _issuer;
		private readonly string _audience;

		public static readonly string REDIS_INSTANCE_NAME = "WhatsAppClone_";
		public static readonly string TOKEN_FAMILY_IDENTIFIER = "family";

		public int RefreshTokenDays { get; init; }
		public int AccessTokenMinutes { get; init; }

		public SecurityUtils(string refreshTokenSignKey, string accessTokenSignKey, int refreshTokenDays, int accessTokenMinutes, string issuer, string audience)
		{
			_refreshTokenSignKey = refreshTokenSignKey;
			_accessTokenSignKey = accessTokenSignKey;
			RefreshTokenDays = refreshTokenDays;
			AccessTokenMinutes = accessTokenMinutes;
			_issuer = issuer;
			_audience = audience;
		}

		public (byte[] Password, byte[] Salt) GeneratePassword(string password)
		{
			var rnd = new Random();
			var salt = new byte[512];
			rnd.NextBytes(salt);
			return (Password: GeneratePassword(password, salt), Salt: salt);
		}

		public byte[] GeneratePassword(string password, byte[] salt)
		{
			return KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA512, 1000, 512);
		}

		public string GenerateRefreshToken(string username, DateTime family)
		{
			return GenerateToken(username, DateTime.UtcNow.AddDays(RefreshTokenDays), family, _refreshTokenSignKey);
		}

		public async Task<TokenValidationResult> ValidateRefreshToken(string refreshTokenString)
		{
			var handler = new JwtSecurityTokenHandler();
			return await handler.ValidateTokenAsync(refreshTokenString, GenerateRefreshTokenValidationParams());
		}

		public string GenerateAccessToken(string username)
		{
			return GenerateToken(username, DateTime.UtcNow.AddMinutes(AccessTokenMinutes), DateTime.Now, _accessTokenSignKey);
		}

		[UseDbContext(typeof(WaDbContext))]
		public async Task<bool> Authenticate(SignInInput input, [ScopedService] WaDbContext context, CancellationToken cancellationToken)
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

		public TokenValidationParameters GenerateRefreshTokenValidationParams()
		{
			return GenerateTokenValidationParams(_refreshTokenSignKey);
		}

		public TokenValidationParameters GenerateAccessTokenValidationParams()
		{
			return GenerateTokenValidationParams(_accessTokenSignKey);
		}

		private TokenValidationParameters GenerateTokenValidationParams(string signKey)
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
				ValidIssuer = _issuer,
				ValidAudience = _audience,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey))
			};
		}

		private string GenerateToken(string username, DateTime expires, DateTime tokenFamily, string signKey)
		{
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
			var claims = new[]
			{
				new Claim(ClaimTypes.NameIdentifier, username),
				new Claim(TOKEN_FAMILY_IDENTIFIER, tokenFamily.ToString())
			};

			var token = new JwtSecurityToken(
				issuer: _issuer,
				audience: _audience,
				claims: claims,
				expires: expires,
				signingCredentials: credentials
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
