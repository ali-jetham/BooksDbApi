using Microsoft.AspNetCore.Authentication.BearerToken;

namespace LifeDbApi.Models.Dto;

public class AuthGoogleResponseDto
{
	public string? AccessToken { get; set; }
	public string? RefreshToken { get; set; }
}
