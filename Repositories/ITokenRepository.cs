using LifeDbApi.Models.Domain;

namespace LifeDbApi.Repositories;

public interface ITokenRepository
{
	Task<RefreshToken> Create(RefreshToken refreshToken);
	Task<RefreshToken?> Get(string refreshToken);
	Task<bool> Delete(RefreshToken refreshToken);
}
