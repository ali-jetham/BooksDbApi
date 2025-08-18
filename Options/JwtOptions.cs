using System;

namespace LifeDbApi.Options;

public class JwtOptions
{
	public const string Jwt = "Jwt";

	public string Issuer { get; init; }
	public string Audience { get; init; }
	public string SecretKey { get; init; }
}
