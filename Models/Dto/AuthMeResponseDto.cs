using System;

namespace LifeDbApi.Models.Dto;

public class AuthMeResponseDto
{
	public string Id { get; set; }
	public bool IsAuthenticated { get; set; }
}
