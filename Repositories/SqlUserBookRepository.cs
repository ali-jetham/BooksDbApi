using LifeDbApi.Data;
using LifeDbApi.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace LifeDbApi.Repositories;

public class SqlUserBookRepository(LifeDbContext dbContext) : IUserBookRepository
{
	private readonly LifeDbContext dbContext = dbContext;

	public async Task<UserBook> Get(Guid userBookId)
	{
		var result = await dbContext
			.UserBooks.Include(ub => ub.Book)
			.FirstOrDefaultAsync(ub => ub.Id == userBookId);
		if (result == null)
		{
			throw new KeyNotFoundException("UserBook not found for the specified id.");
		}
		return result;
	}

	public async Task<List<UserBook>> GetAll(Guid userId)
	{
		var result = dbContext.UserBooks.Include(ub => ub.Book).Where(ub => ub.UserId == userId).ToList();
		return result;
	}

	public async Task<UserBook> Create(UserBook userBook)
	{
		var result = dbContext.UserBooks.Add(userBook);
		await dbContext.SaveChangesAsync();
		return result.Entity;
	}

	public async Task<UserBook> Update(UserBook userBook)
	{
		var result = dbContext.UserBooks.Update(userBook);
		await dbContext.SaveChangesAsync();
		return result.Entity;
	}

	public async Task<bool> Delete(Guid userBookId)
	{
		int rowsAffected = await dbContext.UserBooks.Where(ub => ub.Id == userBookId).ExecuteDeleteAsync();

		if (rowsAffected > 0)
			return true;

		return false;
	}
}
