using System;

namespace LifeDbApi.Models.Domain;

public class Collection
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public string Data { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

	public Guid UserId { get; set; }
	public User User { get; set; }
}
