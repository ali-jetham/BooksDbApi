using LifeDbApi.Models.Domain;
using LifeDbApi.Models.Dto;

namespace LifeDbApi.Mappings;

public static class BookGetResponseMappings
{
	public static BookGetAllResponseDto Map(UserBook userBook)
	{
		return new BookGetAllResponseDto
		{
			UserBookId = userBook.Id,
			Title = userBook.Book.Title,
			CoverUrl = userBook.Book.CoverUrl,
			Status = userBook.Status,
			Rating = userBook.Rating,
			Favorite = userBook.Favorite,
			DateAdded = userBook.DateAdded,
			DateStarted = userBook.DateAdded,
			DateFinished = userBook.DateFinished,
			Isbn10 = userBook.Book.Isbn10,
			Isbn13 = userBook.Book.Isbn13,
			PageCount = userBook.Book.PageCount,
			Edition = userBook.Book.Edition,
			Series = userBook.Book.Series,
			Publisher = userBook.Book.Publisher,
			PublicationDate = userBook.Book.PublicationDate,
			Genre = userBook.Book.Genre,
			Authors = userBook.Book.Authors,
		};
	}
}
