using LifeDbApi.Models.Dto;
using LifeDbApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LifeDbApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TokenController : ControllerBase
	{
		private readonly ILogger<TokenController> logger;
		private readonly TokenService tokenService;

		public TokenController(ILogger<TokenController> logger, TokenService tokenService)
		{
			this.logger = logger;
			this.tokenService = tokenService;
		}

		[HttpGet]
		[Route("refresh")]
		public async Task<ActionResult> RefreshAccessToken()
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
				};
				Response.Cookies.Append("access_token", result.Value, accessCookieOptions);
				Response.Headers.Add("Access-Control-Allow-Origin", "https://localhost:5173");
				Response.Headers.Add("Vary", "Origin");
				return Ok();
			}
			return BadRequest();
		}

		[HttpDelete]
		[Route("revoke")]
		[Authorize]
		public async Task<ActionResult> RevokeRefreshToken()
		{
			string? refreshToken = Request.Cookies["refresh_token"];
			if (refreshToken == null)
			{
				return BadRequest();
			}
			var result = await tokenService.RevokeRefreshToken(refreshToken);
			if (result)
			{
				return Ok(new TokenRevokeResponseDto() { IsRevoked = result });
			}
			return BadRequest();
		}
	}
}
