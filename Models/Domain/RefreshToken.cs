namespace LifeDbApi.Models.Domain;

public class RefreshToken
{
	public Guid Id { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime ExpiresAt { get; set; }
	public required string TokenHash { get; set; }
	public bool Revoked { get; set; } = false;
	public string? UserAgent { get; set; }

	public Guid UserId { get; set; }
}
