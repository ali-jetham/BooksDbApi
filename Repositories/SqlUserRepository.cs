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

	public async Task<Guid?> GetUserIdByRefreshToken(RefreshToken refreshToken)
	{
		var result = await dbContext.RefreshTokens.FirstOrDefaultAsync(rt =>
			rt.Id == refreshToken.Id
		);

		return result?.UserId;
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
