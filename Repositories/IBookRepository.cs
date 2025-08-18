using System;
using LifeDbApi.Models.Domain;

namespace LifeDbApi.Repositories;

public interface IBookRepository
{
	Task<Book> Get(Guid Id);
	Task<Book> Create(Book book);
	Task<List<Book>> Search(string query);
	Task<Book?> SearchIsbn(string isbn, bool isIsbn10);
}
