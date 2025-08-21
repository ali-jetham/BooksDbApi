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
			// TODO: NOT use email as identifier use google's sub
			User? user = await userRepository.GetUserByEmail(payload.Email);
			if (user == null)
			{
				User newUser = await userService.CreateUser(
					payload.Email,
					payload.EmailVerified,
					OAuthProvider.Google,
					payload.Subject,
					payload.Name
				);

				if (newUser == null)
				{
					return Result.Fail("Cannot create user");
				}
				var tokens = await tokenService.IssueTokens(newUser.Id, newUser.Email, newUser.Name);
				return Result.Ok(tokens);
			}
			else if (user != null && user.OAuthProvider == OAuthProvider.Google)
			{
				var tokens = await tokenService.IssueTokens(user.Id, user.Email, user.Name);
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
		var nameClaim = jwt?.Claims.FirstOrDefault(c => c.Type == "name");
		if (subClaim == null || nameClaim == null)
		{
			return Result.Fail(new Error("sub claim not found in token."));
		}

		return Result.Ok(
			new AuthMeResponseDto
			{
				IsAuthenticated = true,
				Id = subClaim.Value,
				Name = nameClaim.Value,
			}
		);
	}
}
