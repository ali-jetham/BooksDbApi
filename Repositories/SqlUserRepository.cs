using LifeDbApi.Data;
using LifeDbApi.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace LifeDbApi.Repositories;

public class SqlUserRepository(LifeDbContext dbContext) : IUserRepository
{
	public async Task<User> CreateUser(User user)
	{
		var result = await dbContext.Users.AddAsync(user);
		await dbContext.SaveChangesAsync();
		return result.Entity;
	}

	public void DeleteUser()
	{
		throw new NotImplementedException();
	}

	public Task<User> GetUserById(Guid id)
	{
		throw new NotImplementedException();
	}

	public async Task<User?> GetUserByRefreshToken(RefreshToken refreshToken)
	{
		User? result = await dbContext
			.Users.Include(u => u.RefreshTokens)
			.FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.TokenHash == refreshToken.TokenHash));

		return result;
	}

	public Task<User> GetUserByEmail(string email)
	{
		var user = dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
		return user;
	}

	public void UpdateUser()
	{
		throw new NotImplementedException();
	}
}
