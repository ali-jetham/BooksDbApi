using System;
using LifeDbApi.Data;
using LifeDbApi.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace LifeDbApi.Repositories;

public class SqlCollectionRepository : ICollectionRepository
{
	private readonly LifeDbContext dbContext;

	public SqlCollectionRepository(LifeDbContext dbContext)
	{
		this.dbContext = dbContext;
	}

	public async Task<List<Collection>> GetCollections(Guid userId)
	{
		var result = await dbContext.Collections.Where(x => x.UserId == userId).ToListAsync();
		return result;
	}
}
