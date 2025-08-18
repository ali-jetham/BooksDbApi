using System;
using FluentResults;
using LifeDbApi.Models.Domain;
using LifeDbApi.Repositories;

namespace LifeDbApi.Services;

public class CollectionService
{
	private readonly ILogger<CollectionService> logger;
	private readonly ICollectionRepository collectionRepository;

	public CollectionService(
		ILogger<CollectionService> logger,
		ICollectionRepository collectionRepository
	)
	{
		this.logger = logger;
		this.collectionRepository = collectionRepository;
	}

	public async Task<Result<List<Collection>>> GetCollections(Guid userId)
	{
		var result = await collectionRepository.GetCollections(userId);
		return Result.Ok(result);
	}
}
