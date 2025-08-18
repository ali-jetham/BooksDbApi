using System;
using LifeDbApi.Models.Domain;
using LifeDbApi.Models.Dto;

namespace LifeDbApi.Mappings;

public static class BookSearchResponseMapping
{
	public static BookSearchResponseDto Map(OpenLibraryBookResponseDto entity)
	{
		return new BookSearchResponseDto
		{
			ExternalId = entity?.Identifiers?.openlibrary?.FirstOrDefault(),
			Type = BookSearchType.OpenLibrary,
			Title = entity?.Title,
			Authors = entity?.Authors?.Select(a => a.name).ToList(),
			CoverUrl = entity?.Cover?.medium,
			Isbn10 = entity?.Identifiers?.isbn_10?.FirstOrDefault(),
			Isbn13 = entity?.Identifiers?.isbn_13?.FirstOrDefault(),
			PageCount = entity?.PageCount,
			Publisher = entity?.Publishers?.Select(p => p.name).ToList(),
			PublicationDate = entity?.PublicationDate,
			Genre = entity?.Subjects?.Take(3).Select(s => s.name).ToList(),
		};
	}

	public static BookSearchResponseDto Map(Book entity)
	{
		return new BookSearchResponseDto
		{
			InternalId = entity.Id,
			ExternalId = entity.ExternalId,
			Type = BookSearchType.Internal,
			Title = entity.Title,
			Authors = entity.Authors,
			CoverUrl = entity.CoverUrl,
			Isbn10 = entity.Isbn10,
			Isbn13 = entity.Isbn13,
			PageCount = entity.PageCount,
			Publisher = entity.Publisher,
			PublicationDate = entity.PublicationDate.ToString(),
			Genre = entity.Genre,
		};
	}
}
