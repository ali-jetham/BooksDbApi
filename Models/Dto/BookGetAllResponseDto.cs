using LifeDbApi.Models.Domain;

namespace LifeDbApi.Models.Dto;

public class BookGetAllResponseDto
{
	public Guid UserBookId { get; set; }
	public required string Title { get; set; }
	public required string CoverUrl { get; set; }
	public required BookStatus Status { get; set; }
	public required int? Rating { get; set; }
	public required bool Favorite { get; set; }
	public DateOnly? DateAdded { get; set; }
	public DateOnly? DateStarted { get; set; }
	public DateOnly? DateFinished { get; set; }
	public string? Isbn10 { get; set; }
	public string? Isbn13 { get; set; }
	public int? PageCount { get; set; }
	public byte? Edition { get; set; }
	public string? Series { get; set; }
	public required List<string> Publisher { get; set; }
	public DateOnly? PublicationDate { get; set; }
	public required List<string> Genre { get; set; }
	public required List<string> Authors { get; set; }
}
