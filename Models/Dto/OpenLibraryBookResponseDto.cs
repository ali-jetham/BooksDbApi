using System.Text.Json.Serialization;

namespace LifeDbApi.Models.Dto;

public class OpenLibraryBookResponseDto
{
	[JsonPropertyName("title")]
	public string Title { get; set; }

	[JsonPropertyName("authors")]
	public List<Author> Authors { get; set; }

	[JsonPropertyName("cover")]
	public Cover Cover { get; set; }

	[JsonPropertyName("identifiers")]
	public Identifiers Identifiers { get; set; }

	[JsonPropertyName("number_of_pages")]
	public int PageCount { get; set; }

	[JsonPropertyName("publishers")]
	public List<Publisher> Publishers { get; set; }

	[JsonPropertyName("publish_date")]
	public string PublicationDate { get; set; }

	[JsonPropertyName("subjects")]
	public List<Subjects> Subjects { get; set; }
}

public class Publisher
{
	public string name { get; set; }
}

public class Author
{
	public string name { get; set; }
}

public class Cover
{
	public string medium { get; set; }
}

public class Identifiers
{
	public List<string> isbn_10 { get; set; }
	public List<string> isbn_13 { get; set; }
	public List<string> openlibrary { get; set; }
}

public class Subjects
{
	public string name { get; set; }
}
