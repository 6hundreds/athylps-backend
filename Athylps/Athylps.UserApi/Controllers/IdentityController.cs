using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Athylps.UserApi.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class IdentityController : ControllerBase
	{
		// GET api/values
		[HttpGet]
		public IActionResult Get()
		{
			List<Claim> claims = User.Claims
				.ToList();

			return new JsonResult(claims);
		}
	}
}
