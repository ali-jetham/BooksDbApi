namespace LifeDbApi.Models.Dto;

public class BookCreateRequestDto : BookSearchResponseDto
{
	// public Guid UserId { get; set; }
	public DateOnly DateAdded { get; set; }
}
