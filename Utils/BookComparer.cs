using System;
using LifeDbApi.Models.Domain;

namespace LifeDbApi.Utils;

public class BookComparer : IEqualityComparer<Book>
{
	public bool Equals(Book x, Book y) => x.Id == y.Id;

	public int GetHashCode(Book obj) => obj.Id.GetHashCode();
}
