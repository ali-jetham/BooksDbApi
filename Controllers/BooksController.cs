using LifeDbApi.Models.Dto;
using LifeDbApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LifeDbApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BooksController(BookService bookService) : ControllerBase
	{
		private readonly BookService bookService = bookService;

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> GetAll()
		{
			var token = Request.Cookies["access_token"];
			if (token == null)
			{
				return Unauthorized();
			}
			Guid? userId = TokenService.GetIdFromToken(token);
			if (userId == null)
			{
				return BadRequest("User does not exist");
			}
			List<BookGetAllResponseDto> books = await bookService.GetAllUserBooks(userId.Value);
			return Ok(books);
		}

		[HttpGet]
		[Route("{id}")]
		[Authorize]
		public async Task<ActionResult> Get(Guid id)
		{
			var token = Request.Cookies["access_token"];
			Guid? userIdFromToken = TokenService.GetIdFromToken(token!);
			if (userIdFromToken == null)
			{
				return BadRequest("User does not exist");
			}
			(Guid userId, BookGetResponseDto responseDto) = await bookService.GetUserBook(id);
			if (userIdFromToken != userId)
			{
				return Forbid();
			}
			return Ok(responseDto);
		}

		[HttpGet]
		[Authorize]
		[Route("search")]
		public async Task<ActionResult> Search([FromQuery] string q)
		{
			var origin = Request.Headers["Origin"].ToString();
			if (!string.IsNullOrEmpty(origin))
			{
				return BadRequest();
			}
			var result = await bookService.Search(q);
			return Ok(result);
		}

		[HttpPost]
		[Authorize]
		public async Task<ActionResult> Create([FromBody] BookCreateRequestDto requestDto)
		{
			var accessToken = Request.Cookies["access_token"];
			if (accessToken == null)
			{
				return Unauthorized();
			}
			if (requestDto.InternalId == null && requestDto.ExternalId == null)
			{
				return BadRequest("Both InternalId AND ExternalId cannot be null");
			}
			Guid? userId = TokenService.GetIdFromToken(accessToken);
			if (userId == null)
			{
				return BadRequest("User does not exist");
			}
			var result = await bookService.CreateUserBook(userId.Value, requestDto);
			return Ok(result);
		}

		[HttpPatch("{userBookId}")]
		[Authorize]
		public async Task<ActionResult> Update(Guid userBookId, UserBookUpdateRequestDto requestDto)
		{
			// TODO: check if the userId and resource Id matches before updating
			var updatedUserBook = await bookService.UpdateUserBook(userBookId, requestDto);
			if (updatedUserBook == null)
			{
				return BadRequest();
			}
			// FIX: change this return type, currently too much info is sent back
			return Ok(updatedUserBook);
		}

		[HttpDelete("{userBookId}")]
		[Authorize]
		public async Task<ActionResult> Delete(Guid userBookId)
		{
			bool result = await bookService.DeleteUserBook(userBookId);
			return Ok(result);
		}
	}
}
