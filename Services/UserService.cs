using FluentResults;
using LifeDbApi.Models.Domain;
using LifeDbApi.Repositories;

namespace LifeDbApi.Services;

public class UserService(ILogger<UserService> logger, IUserRepository userRepository)
{
	private readonly ILogger logger = logger;
	private readonly IUserRepository userRepository = userRepository;

	public async Task<User> CreateUser(
		string email,
		bool emailVerified,
		OAuthProvider oAuthProvider,
		string sub,
		string name
	)
	{
		User user = new()
		{
			Id = Guid.NewGuid(),
			Name = name,
			Email = email,
			EmailVerified = emailVerified,
			OAuthProvider = oAuthProvider,
			Sub = sub,
		};
		var createdUser = await userRepository.CreateUser(user);
		return createdUser;
	}
}
