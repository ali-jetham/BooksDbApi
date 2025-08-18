using System;
using LifeDbApi.Models.Domain;

namespace LifeDbApi.Repositories;

public interface ICollectionRepository
{
	Task<List<Collection>> GetCollections(Guid userId);
	// Collection CreateCollection(Guid userId);
}
