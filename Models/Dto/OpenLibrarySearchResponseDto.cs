using System;
using System.Text.Json.Serialization;

namespace LifeDbApi.Models.Dto;

public class OpenLibrarySearchResponseDto
{
	[JsonPropertyName("docs")]
	public List<Docs> Docs { get; set; }
}

public class Docs
{
	[JsonPropertyName("cover_edition_key")]
	public string Key { get; set; }
}
