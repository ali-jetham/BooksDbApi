using Microsoft.AspNetCore.Mvc;

namespace LifeDbApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TestController : ControllerBase
	{
		[HttpGet]
		public ActionResult Get()
		{
			return Content(
				"""
				<head>
					<title>Redirecting to Another page in HTML</title>
					<meta http-equiv="refresh" content="5; url =https://www.geeksforgeeks.org/community/" />
				</head>
				<body>
				Auth successful, redirecting in 5s
				</body>
				""",
				"text/html"
			);
		}
	}
}
