using System;
using LifeDbApi.Data;
using LifeDbApi.Models.Domain;
using LifeDbApi.Utils;
using Microsoft.EntityFrameworkCore;

namespace LifeDbApi.Repositories;

public class SqlBookRepository(LifeDbContext dbContext) : IBookRepository
{
	private readonly LifeDbContext dbContext = dbContext;

	/// <summary>
	///
	/// </summary>
	/// <param name="book"></param>
	/// <returns></returns>
	public async Task<Book> Create(Book book)
	{
		bool exists = await dbContext.Books.AnyAsync(b =>
			b.Isbn10 == book.Isbn10 || b.Isbn13 == book.Isbn13
		);
		if (exists)
		{
			throw new InvalidOperationException("A book with the same ISBN already exists.");
		}
		var result = await dbContext.Books.AddAsync(book);
		await dbContext.SaveChangesAsync();
		return result.Entity;
	}

	public async Task<List<Book>> Search(string query)
	{
		List<Book> containsBooks = await dbContext
			.Books.Where(b => b.Title.ToLower().Contains(query))
			.Take(10)
			.ToListAsync();

		List<Book> startsWithBooks = await dbContext
			.Books.Where(b => b.Title.ToLower().StartsWith(query))
			.Take(10)
			.ToListAsync();

		containsBooks.AddRange(startsWithBooks);
		return containsBooks.Distinct(new BookComparer()).ToList();
	}

	public async Task<Book?> SearchIsbn(string isbn, bool isIsbn10)
	{
		if (isIsbn10)
			return await dbContext.Books.FirstOrDefaultAsync(b => b.Isbn10 == isbn);
		else
			return await dbContext.Books.FirstOrDefaultAsync(b => b.Isbn13 == isbn);
	}

	public async Task<Book?> Get(Guid id)
	{
		return await dbContext.Books.FirstOrDefaultAsync(b => b.Id == id);
	}
}
