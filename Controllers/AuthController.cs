using LifeDbApi.Models.Dto;
using LifeDbApi.Options;
using LifeDbApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LifeDbApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
	private readonly ILogger<AuthController> logger;
	private readonly AuthService authService;
	private readonly TokenService tokenService;
	private readonly FrontendOptions frontendOptions;

	public AuthController(
		ILogger<AuthController> logger,
		IOptions<FrontendOptions> frontendOptions,
		AuthService authService,
		TokenService tokenService
	)
	{
		this.logger = logger;
		this.frontendOptions = frontendOptions.Value;
		this.authService = authService;
		this.tokenService = tokenService;
	}

	[HttpPost]
	[Route("google")]
	public async Task<ActionResult> Google([FromForm] string credential)
	{
		var result = await authService.SignInWithGoogle(credential);

		if (result.IsFailed)
		{
			return BadRequest();
		}
		else if (result.IsSuccess)
		{
			var refreshCookieOptions = new CookieOptions
			{
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.None,
				Path = "/api/token",
				Expires = DateTime.UtcNow.AddDays(30),
			};
			var accessCookieOptions = new CookieOptions
			{
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.None,
				Path = "/api",
				Expires = DateTime.UtcNow.AddMinutes(15),
			};
			Response.Cookies.Append("refresh_token", result.Value.RefreshToken!, refreshCookieOptions);
			Response.Cookies.Append("access_token", result.Value.AccessToken!, accessCookieOptions);

			return Content(
				$"""
				<head>
					<title>Redirecting to Another page in HTML</title>
					<meta http-equiv="refresh" content="5; url = {frontendOptions.Url}/app/books" />
				</head>
				<body>
				Auth successful, redirecting in 5s
				</body>
				""",
				"text/html"
			);
		}
		else
		{
			return BadRequest();
		}
	}

	[HttpGet]
	[Route("refresh")]
	public async Task<ActionResult> Refresh()
	{
		var refreshToken = Request.Cookies["refresh_token"];
		if (refreshToken == null)
		{
			return Unauthorized("Invalid refresh token");
		}
		var result = await tokenService.IssueAccessToken(refreshToken);

		if (result.IsFailed)
		{
			return Unauthorized();
		}
		else if (result.Value != null)
		{
			// TODO: try to reuse this code from AuthController
			var accessCookieOptions = new CookieOptions
			{
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.None,
				Path = "/api",
				Expires = DateTime.UtcNow.AddMinutes(15),
			};
			Response.Cookies.Append("access_token", result.Value, accessCookieOptions);
			return Ok();
		}
		return BadRequest();
	}

	[HttpDelete]
	[Route("logout")]
	[Authorize]
	public async Task<ActionResult> Logout()
	{
		string? refreshToken = Request.Cookies["refresh_token"];
		if (refreshToken == null)
		{
			return BadRequest("Refresh token expired");
		}
		var result = await tokenService.RevokeRefreshToken(refreshToken);
		if (!result)
		{
			return BadRequest("Failed to delete refresh token");
		}
		Response.Cookies.Append(
			"access_token",
			"",
			new CookieOptions
			{
				Expires = DateTime.UtcNow.AddDays(-1),
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.None,
				Path = "/api",
			}
		);
		Response.Cookies.Append(
			"refresh_token",
			"",
			new CookieOptions
			{
				Expires = DateTime.UtcNow.AddDays(-1),
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.None,
				Path = "/api/auth",
			}
		);
		return Ok(new TokenRevokeResponseDto() { IsRevoked = result });
	}

	[HttpGet]
	[Route("me")]
	[Authorize]
	public async Task<ActionResult> Me()
	{
		var result = authService.AuthMe(Request.Cookies["access_token"]);
		if (result.IsFailed)
		{
			return Unauthorized();
		}
		return Ok(result.Value);
	}
}
