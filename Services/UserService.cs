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
		string sub
	)
	{
		User user = new()
		{
			Id = Guid.NewGuid(),
			Name = email.Split("@")[0],
			Email = email,
			EmailVerified = emailVerified,
			OAuthProvider = oAuthProvider,
			Sub = sub,
		};
		var createdUser = await userRepository.CreateUser(user);
		return createdUser;
	}
}
