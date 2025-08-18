using LifeDbApi.Models.Domain;

namespace LifeDbApi.Repositories;

public interface IUserRepository
{
	Task<User> GetUserById(Guid id);
	Task<Guid?> GetUserIdByRefreshToken(RefreshToken refreshToken);
	Task<User> GetUserByEmail(string email);
	Task<User> CreateUser(User user);
	void UpdateUser();
	void DeleteUser();
}
