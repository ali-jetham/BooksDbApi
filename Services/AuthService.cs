using FluentResults;
using Google.Apis.Auth;
using LifeDbApi.Models.Domain;
using LifeDbApi.Models.Dto;
using LifeDbApi.Repositories;
using Microsoft.IdentityModel.JsonWebTokens;

namespace LifeDbApi.Services;

public class AuthService(
	IUserRepository userRepository,
	ILogger<AuthService> logger,
	UserService userService,
	TokenService refreshTokenService
)
{
	private readonly ILogger<AuthService> logger = logger;
	private readonly IUserRepository userRepository = userRepository;
	private readonly TokenService tokenService = refreshTokenService;

	public async Task<Result<AuthGoogleResponseDto>> SignInWithGoogle(string credential)
	{
		try
		{
			var payload = await GoogleJsonWebSignature.ValidateAsync(credential);
			var user = await userRepository.GetUserByEmail(payload.Email);
			if (user == null)
			{
				var newUser = await userService.CreateUser(
					payload.Email,
					payload.EmailVerified,
					OAuthProvider.Google,
					payload.Subject
				);

				if (newUser == null)
				{
					return Result.Fail("Cannot create user");
				}
				var tokens = await tokenService.GenerateTokens(newUser.Id, payload.Email);
				return Result.Ok(tokens);
			}
			else if (user != null && user.OAuthProvider == OAuthProvider.Google)
			{
				var tokens = await tokenService.GenerateTokens(user.Id, payload.Email);
				return Result.Ok(tokens);
			}
			else
			{
				return Result.Fail("User exists with another OIDC Provider");
			}
		}
		catch (InvalidJwtException e)
		{
			logger.LogError(e.StackTrace);
			return Result.Fail(e.Message);
		}
	}

	public Result<AuthMeResponseDto> AuthMe(string accessToken)
	{
		if (accessToken == null)
		{
			return Result.Fail(new Error("access token not found"));
		}

		var handler = new JsonWebTokenHandler();
		var jwt = handler.ReadJsonWebToken(accessToken);
		var subClaim = jwt?.Claims.FirstOrDefault(c => c.Type == "sub");
		if (subClaim == null)
		{
			return Result.Fail(new Error("sub claim not found in token."));
		}

		return Result.Ok(new AuthMeResponseDto { IsAuthenticated = true, Id = subClaim.Value });
	}
}
