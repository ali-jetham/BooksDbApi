using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FluentResults;
using LifeDbApi.Models.Domain;
using LifeDbApi.Models.Dto;
using LifeDbApi.Repositories;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace LifeDbApi.Services;

public class TokenService(
	IConfiguration configuration,
	ITokenRepository tokenRepository,
	IUserRepository userRepository
)
{
	private readonly IConfiguration configuration = configuration;
	private readonly ITokenRepository tokenRepository = tokenRepository;
	private readonly IUserRepository userRepository = userRepository;

	public async Task<string> CreateRefreshToken(Guid userId)
	{
		var bytes = new byte[30];
		RandomNumberGenerator.Fill(bytes);
		string token = Convert.ToBase64String(bytes);
		var bytesHash = SHA256.HashData(bytes);
		var tokenHash = Convert.ToBase64String(bytesHash);

		var refreshToken = new RefreshToken
		{
			Id = Guid.NewGuid(),
			TokenHash = tokenHash,
			CreatedAt = DateTime.UtcNow,
			ExpiresAt = DateTime.UtcNow.AddDays(30),
			UserId = userId,
		};
		await tokenRepository.Create(refreshToken);
		return token;
	}

	private string CreateAccessToken(string id)
	{
		// TODO: replace with options pattern using env vars in prod (not now)
		string secretKey = configuration["Jwt:Secret"];
		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(
				[
					// new Claim(JwtRegisteredClaimNames.Email, email),
					new Claim(JwtRegisteredClaimNames.Sub, id),
				]
			),
			Expires = DateTime.UtcNow.AddMinutes(20),
			SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256),
			Issuer = configuration["Jwt:Issuer"],
			Audience = configuration["Jwt:Audience"],
		};
		var handler = new JsonWebTokenHandler();
		string token = handler.CreateToken(tokenDescriptor);
		return token;
	}

	public async Task<AuthGoogleResponseDto> GenerateTokens(Guid id, string email)
	{
		var refreshToken = await CreateRefreshToken(id);
		string accessToken = CreateAccessToken(id.ToString());
		var tokens = new AuthGoogleResponseDto { AccessToken = accessToken, RefreshToken = refreshToken };
		return tokens;
	}

	public async Task<Result<string>> IssueAccessToken(string refreshToken)
	{
		RefreshToken? result = await GetRefreshToken(refreshToken);

		if (result == null || !ValidateRefreshToken(result))
		{
			return Result.Fail("Refresh token has been revoked or expired");
		}
		Guid? userId = await userRepository.GetUserIdByRefreshToken(result);
		if (userId == null)
		{
			return Result.Fail("");
		}
		var newAccessToken = CreateAccessToken(userId.ToString()!);
		return Result.Ok(newAccessToken);
	}

	private async Task<RefreshToken?> GetRefreshToken(string refreshToken)
	{
		byte[] tokenBytes = Convert.FromBase64String(refreshToken);
		byte[] bytesHash = SHA256.HashData(tokenBytes);
		var token = Convert.ToBase64String(bytesHash);
		RefreshToken? result = await tokenRepository.Get(token);
		return result;
	}

	public async Task<bool> RevokeRefreshToken(string refreshTokenHash)
	{
		RefreshToken? refreshToken = await GetRefreshToken(refreshTokenHash);
		if (refreshToken == null)
		{
			return false;
		}
		var result = await tokenRepository.Delete(refreshToken);
		if (result)
		{
			return true;
		}
		return false;
	}

	public bool ValidateRefreshToken(RefreshToken token)
	{
		int result = DateTime.Compare(token.ExpiresAt, DateTime.UtcNow);
		if (token.Revoked || result < 0)
		{
			return false;
		}
		return true;
	}

	public static Guid? GetIdFromToken(string accessToken)
	{
		var handler = new JsonWebTokenHandler();
		var jwt = handler.ReadJsonWebToken(accessToken);
		var subClaim = jwt?.Claims.FirstOrDefault(c => c.Type == "sub");
		if (subClaim != null && Guid.TryParse(subClaim.Value, out Guid userId))
		{
			return userId;
		}
		return null;
	}
}
