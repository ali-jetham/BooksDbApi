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
	private readonly FrontendOptions frontendOptions;

	public AuthController(
		ILogger<AuthController> logger,
		IOptions<FrontendOptions> frontendOptions,
		AuthService authService
	)
	{
		this.logger = logger;
		this.frontendOptions = frontendOptions.Value;
		this.authService = authService;
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
				// TODO: change this values in production
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
