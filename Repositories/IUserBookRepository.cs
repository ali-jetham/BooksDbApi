using LifeDbApi.Models.Domain;

namespace LifeDbApi.Repositories;

public interface IUserBookRepository
{
	Task<UserBook> Get(Guid userBookId);
	Task<List<UserBook>> GetAll(Guid userId);
	Task<UserBook> Create(UserBook userBook);
	Task<UserBook> Update(UserBook userBook);
	Task<bool> Delete(Guid userBookId);
}
