using LifeDbApi.Data;
using LifeDbApi.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace LifeDbApi.Repositories;

public class SqlTokenRepository(LifeDbContext dbContext) : ITokenRepository
{
	private readonly LifeDbContext dbContext = dbContext;

	public async Task<RefreshToken> Create(RefreshToken refreshToken)
	{
		var result = await dbContext.RefreshTokens.AddAsync(refreshToken);
		await dbContext.SaveChangesAsync();
		return result.Entity;
	}

	public async Task<RefreshToken?> Get(string refreshToken)
	{
		var result = await dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == refreshToken);
		return result;
	}

	public async Task<bool> Delete(RefreshToken refreshToken)
	{
		var result = dbContext.RefreshTokens.Remove(refreshToken);
		await dbContext.SaveChangesAsync();
		return true;
	}
}
