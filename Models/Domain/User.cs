using System.ComponentModel.DataAnnotations;

namespace LifeDbApi.Models.Domain;

public enum OAuthProvider
{
	Google,
	Facebook,
	Apple,
	Microsoft,
}

public class User
{
	public required Guid Id { get; set; }

	[StringLength(128)]
	public required string Name { get; set; }

	[StringLength(256)]
	[Required]
	public required string Email { get; set; }

	public bool EmailVerified { get; set; }
	public OAuthProvider OAuthProvider { get; set; }

	[StringLength(256)]
	public string? Sub { get; set; }

	public List<RefreshToken> RefreshTokens { get; set; }
	public List<Collection> Collections { get; set; }

	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
