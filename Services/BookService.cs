using System.Text.Json;
using LifeDbApi.Mappings;
using LifeDbApi.Models.Domain;
using LifeDbApi.Models.Dto;
using LifeDbApi.Repositories;

namespace LifeDbApi.Services;

public class BookService(
	ILogger<BookService> logger,
	HttpClient httpClient,
	IBookRepository bookRepository,
	IUserBookRepository userBookRepository
)
{
	private readonly ILogger<BookService> logger = logger;
	private readonly HttpClient httpClient = httpClient;
	private readonly IBookRepository bookRepository = bookRepository;
	private readonly IUserBookRepository userBookRepository = userBookRepository;

	/// <summary>
	/// Get all books by User.Id
	/// </summary>
	/// <param name="userId"></param>
	/// <returns></returns>
	public async Task<List<BookGetAllResponseDto>> GetAllUserBooks(Guid userId)
	{
		List<UserBook> userBooks = await userBookRepository.GetAll(userId);
		var bookDtos = new List<BookGetAllResponseDto>();
		foreach (UserBook userBook in userBooks)
		{
			BookGetAllResponseDto bookDto = BookGetResponseMappings.Map(userBook);
			bookDtos.Add(bookDto);
		}
		return bookDtos;
	}

	/// <summary>
	///
	/// </summary>
	/// <param name="userBookId"></param>
	/// <returns></returns>
	public async Task<(Guid, BookGetResponseDto)> GetUserBook(Guid userBookId)
	{
		UserBook userBook = await userBookRepository.Get(userBookId);
		BookGetResponseDto response = new()
		{
			UserBookId = userBook.Id,
			Title = userBook.Book?.Title ?? string.Empty,
			CoverUrl = userBook.Book?.CoverUrl ?? string.Empty,
			Status = userBook.Status,
			Rating = userBook.Rating,
			Favorite = userBook.Favorite,
			DateAdded = userBook.DateAdded,
			DateStarted = userBook.DateStarted,
			DateFinished = userBook.DateFinished,
			Isbn10 = userBook.Book?.Isbn10,
			Isbn13 = userBook.Book?.Isbn13,
			Edition = userBook.Book?.Edition,
			Series = userBook.Book?.Series,
			Publisher = userBook.Book?.Publisher,
			PublicationDate = userBook.Book?.PublicationDate,
			Genre = userBook.Book.Genre,
			Authors = userBook.Book?.Authors,
			Notes = userBook.Notes,
		};
		return (userBook.UserId, response);
	}

	/// <summary>
	/// Search for books by title or ISBN.
	/// </summary>
	/// <param name="query"> Book title or ISBN</param>
	/// <returns></returns>
	public async Task<List<BookSearchResponseDto>> Search(string query)
	{
		logger.LogDebug(query);
		bool isIsbn10 = IsIsbn10(query);
		bool isIsbn13 = IsIsbn13(query);

		if (isIsbn10 || isIsbn13)
		{
			return await SearchIsbn(query, isIsbn10);
		}
		else
		{
			return await SearchQuery(query);
		}
	}

	/// <summary>
	///
	/// </summary>
	/// <returns></returns>
	private async Task<List<BookSearchResponseDto>> SearchQuery(string query)
	{
		List<Book> books = await bookRepository.Search(query.ToLower());
		var internalBooks = new List<BookSearchResponseDto> { };
		foreach (Book book in books)
		{
			internalBooks.Add(BookSearchResponseMapping.Map(book));
		}
		if (internalBooks.Count <= 10)
		{
			string q = Uri.EscapeDataString($"title:{query} language:eng");
			string url = $"https://openlibrary.org/search.json?q={q}&sort=want_to_read&mode=everything";

			var response = await httpClient.GetAsync(url);
			var jsonString = await response.Content.ReadAsStringAsync();
			var searchJson = JsonSerializer.Deserialize<OpenLibrarySearchResponseDto>(jsonString);
			if (searchJson == null)
			{
				return new List<BookSearchResponseDto>();
			}
			var keys = searchJson
				.Docs?.Where(doc => !string.IsNullOrEmpty(doc.Key))
				.Select(doc => $"OLID:{doc.Key}")
				.Take(20);
			string joinedKeys = string.Join(",", keys);
			url = $"api/books?bibkeys={joinedKeys}&format=json&jscmd=data";
			response = await httpClient.GetAsync(url);
			jsonString = await response.Content.ReadAsStringAsync();
			var booksJson = JsonSerializer.Deserialize<Dictionary<string, OpenLibraryBookResponseDto>>(
				jsonString
			);
			List<BookSearchResponseDto> externalBooks = booksJson
				?.Select(kvp => BookSearchResponseMapping.Map(kvp.Value))
				.ToList();
			var filteredExternalBooks = externalBooks
				.Where(externalBook =>
					!internalBooks.Any(internalBook =>
						// Match by ISBN first (most reliable)
						(
							(
								!string.IsNullOrEmpty(externalBook.Isbn10)
								&& !string.IsNullOrEmpty(internalBook.Isbn10)
								&& externalBook.Isbn10 == internalBook.Isbn10
							)
							|| (
								!string.IsNullOrEmpty(externalBook.Isbn13)
								&& !string.IsNullOrEmpty(internalBook.Isbn13)
								&& externalBook.Isbn13 == internalBook.Isbn13
							)
							||
							// If no ISBN match, compare by title
							(
								!string.IsNullOrEmpty(externalBook.Title)
								&& !string.IsNullOrEmpty(internalBook.Title)
								&& externalBook.Title.Equals(internalBook.Title, StringComparison.OrdinalIgnoreCase)
							)
						)
					)
				)
				.ToList();
			internalBooks.AddRange(filteredExternalBooks);
		}
		return internalBooks;
	}

	/// <summary>
	///
	/// </summary>
	/// <param name="isbn"></param>
	/// <param name="isIsbn10"></param>
	/// <returns></returns>
	private async Task<List<BookSearchResponseDto>> SearchIsbn(string isbn, bool isIsbn10)
	{
		Book? book = await bookRepository.SearchIsbn(isbn, isIsbn10);
		if (book != null)
		{
			List<BookSearchResponseDto> searchResponse = [BookSearchResponseMapping.Map(book)];
			return searchResponse;
		}
		else
		{
			string url = $"api/books?bibkeys=ISBN:{isbn}&format=json&jscmd=data";
			var response = await httpClient.GetAsync(url);
			if (!response.IsSuccessStatusCode) { } // TODO: handle when get fails
			var jsonString = await response.Content.ReadAsStringAsync();
			var dict = JsonSerializer.Deserialize<Dictionary<string, OpenLibraryBookResponseDto>>(jsonString);
			var json = dict?.Values.FirstOrDefault();

			var searchResponse = new List<BookSearchResponseDto> { BookSearchResponseMapping.Map(json) };
			return searchResponse;
		}
	}

	public async Task<BookGetAllResponseDto> CreateUserBook(Guid userId, BookCreateRequestDto createDto)
	{
		Guid bookId;
		Book book;
		if (createDto.InternalId == null)
		{
			book = await CreateBook(createDto);
			bookId = book.Id;
		}
		else
		{
			bookId = createDto.InternalId.Value;
			book = await bookRepository.Get(createDto.InternalId.Value);
		}
		var userBook = new UserBook
		{
			Id = Guid.NewGuid(),
			UserId = userId,
			BookId = bookId,
			Status = BookStatus.ToRead,
			DateAdded = createDto.DateAdded,
		};
		UserBook createdUserBook = await userBookRepository.Create(userBook);

		var response = new BookGetAllResponseDto
		{
			Title = book.Title,
			CoverUrl = book.CoverUrl,
			Status = createdUserBook.Status,
			Rating = createdUserBook.Rating,
			Favorite = createdUserBook.Favorite,
			DateStarted = createdUserBook.DateAdded,
			DateFinished = createdUserBook.DateFinished,
			Isbn10 = book.Isbn10,
			Isbn13 = book.Isbn13,
			Edition = book.Edition,
			Series = book.Series,
			Publisher = book.Publisher,
			PublicationDate = book.PublicationDate,
			Genre = book.Genre,
			Authors = book.Authors,
		};
		return response;
	}

	public async Task<Book> CreateBook(BookCreateRequestDto bookCreateDto)
	{
		Book book = new()
		{
			Id = Guid.NewGuid(),
			ExternalId = bookCreateDto.ExternalId,
			Source = BookSource.OpenLibrary,
			Isbn10 = bookCreateDto.Isbn10,
			Isbn13 = bookCreateDto.Isbn13,
			Genre = bookCreateDto.Genre,
			Title = bookCreateDto.Title,
			Authors = bookCreateDto.Authors,
			PageCount = bookCreateDto.PageCount,
			PublicationDate = DateOnly.TryParse(bookCreateDto.PublicationDate, out var d) ? d : null,
			PublicationDateRaw = bookCreateDto.PublicationDate,
			Publisher = bookCreateDto.Publisher,
			// Description = ,
			CoverUrl = bookCreateDto.CoverUrl,
			// Edition =
		};
		Book addedBook = await bookRepository.Create(book);
		return addedBook;
	}

	public async Task<UserBookUpdateRequestDto> UpdateUserBook(
		Guid userBookId,
		UserBookUpdateRequestDto updateDto
	)
	{
		UserBook userBook = await userBookRepository.Get(userBookId);

		if (updateDto.Status.HasValue)
			userBook.Status = updateDto.Status.Value;
		if (updateDto.Rating.HasValue)
			userBook.Rating = updateDto.Rating;
		if (updateDto.DateAdded.HasValue)
			userBook.DateAdded = updateDto.DateAdded;
		if (updateDto.DateStarted.HasValue)
			userBook.DateStarted = updateDto.DateStarted;
		if (updateDto.DateFinished.HasValue)
			userBook.DateFinished = updateDto.DateFinished;
		if (!string.IsNullOrEmpty(updateDto.Notes))
			userBook.Notes = updateDto.Notes;
		if (updateDto.Favorite.HasValue)
			userBook.Favorite = updateDto.Favorite.Value;

		UserBook updatedUserBook = await userBookRepository.Update(userBook);
		UserBookUpdateRequestDto updatedDto = new()
		{
			Status = updatedUserBook.Status,
			Rating = updatedUserBook.Rating,
			DateAdded = updatedUserBook.DateAdded,
			DateStarted = updatedUserBook.DateStarted,
			DateFinished = updatedUserBook.DateFinished,
			Notes = updatedUserBook.Notes,
			Favorite = updatedUserBook.Favorite,
		};
		return updatedDto;
	}

	public async Task<bool> DeleteUserBook(Guid id)
	{
		bool success = await userBookRepository.Delete(id);
		return success;
	}

	/// <summary>
	///
	/// </summary>
	/// <param name="s"></param>
	/// <returns></returns>
	private bool IsIsbn10(string s)
	{
		if (s.Length != 10)
		{
			return false;
		}
		foreach (char c in s[..9])
		{
			if (!char.IsDigit(c))
				return false;
		}
		char last = s[9];
		return char.IsDigit(last) || last == 'X' || last == 'x';
	}

	/// <summary>
	///
	/// </summary>
	/// <param name="s"></param>
	/// <returns></returns>
	private bool IsIsbn13(string s)
	{
		if (s.Length != 13)
			return false;

		foreach (char c in s)
		{
			if (!char.IsDigit(c))
				return false;
		}
		return true;
	}
}
