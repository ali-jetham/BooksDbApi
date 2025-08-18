namespace LifeDbApi.Models.Dto;

public enum BookSearchType
{
	Internal,
	OpenLibrary,
}

public class BookSearchResponseDto
{
	public Guid? InternalId { get; set; }
	public BookSearchType Type { get; set; }
	public required string ExternalId { get; set; }
	public string? Title { get; set; }
	public List<string> Authors { get; set; }
	public string? CoverUrl { get; set; }
	public string? Isbn10 { get; set; }
	public string? Isbn13 { get; set; }
	public int? PageCount { get; set; }
	public List<string> Publisher { get; set; }
	public string? PublicationDate { get; set; }
	public List<string> Genre { get; set; }
}
