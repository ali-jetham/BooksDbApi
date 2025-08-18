namespace LifeDbApi.Models.Domain;

public enum BookSource
{
	Google,
	Goodreads,
	OpenLibrary,
}

// TODO: find a way to add description, series and edition from external sources
public class Book
{
	public required Guid Id { get; set; }
	public required string ExternalId { get; set; }
	public required BookSource Source { get; set; }
	public string? Isbn10 { get; set; }
	public string? Isbn13 { get; set; }
	public List<string> Genre { get; set; }
	public string Title { get; set; }
	public List<string> Authors { get; set; }
	public int? PageCount { get; set; }
	public DateOnly? PublicationDate { get; set; }
	public string PublicationDateRaw { get; set; }
	public List<string> Publisher { get; set; }
	public string? Description { get; set; }
	public string CoverUrl { get; set; }
	public string? Series { get; set; }
	public byte? Edition { get; set; }

	public List<UserBook> UserBooks { get; set; } = [];
}
