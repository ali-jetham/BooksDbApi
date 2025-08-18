namespace LifeDbApi.Models.Domain;

public enum BookStatus
{
	ToRead,
	Reading,
	Read,
	Paused,
	Abandoned,
}

public class UserBook
{
	public Guid Id { get; set; }

	public Guid UserId { get; set; }
	public Guid BookId { get; set; }

	public Book Book { get; set; }
	public User User { get; set; }

	public BookStatus Status { get; set; }
	public int? Rating { get; set; }
	public DateOnly? DateAdded { get; set; }
	public DateOnly? DateStarted { get; set; }
	public DateOnly? DateFinished { get; set; }
	public string? Notes { get; set; }
	public bool Favorite { get; set; }
}
