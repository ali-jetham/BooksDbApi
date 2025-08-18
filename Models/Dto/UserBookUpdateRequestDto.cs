using LifeDbApi.Models.Domain;

namespace LifeDbApi.Models.Dto;

public class UserBookUpdateRequestDto
{
	public BookStatus? Status { get; set; }
	public int? Rating { get; set; }
	public DateOnly? DateAdded { get; set; }
	public DateOnly? DateStarted { get; set; }
	public DateOnly? DateFinished { get; set; }
	public string? Notes { get; set; }
	public bool? Favorite { get; set; }
}
