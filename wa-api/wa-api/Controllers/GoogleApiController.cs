using Microsoft.AspNetCore.Mvc;

namespace wa_api.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class GoogleApiController : ControllerBase
	{
		[HttpGet]
		public ActionResult<string> Get()
		{
			return "Hey";
		}
	}
}
