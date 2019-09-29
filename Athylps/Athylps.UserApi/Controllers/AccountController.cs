using System.Threading.Tasks;
using Athylps.Core.Entities;
using Athylps.Core.Services;
using Athylps.UserApi.Extensions;
using Athylps.UserApi.Models.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Athylps.UserApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly UserManager<User> _userManager;
		private readonly IEmailService _emailService;

		public AccountController(UserManager<User> userManager, IEmailService emailService)
		{
			_userManager = userManager;
			_emailService = emailService;
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> Register(RegisterModel model)
		{
			User user = new User
			{
				UserName = model.UserName,
				Email = model.Email
			};

			IdentityResult result = await _userManager.CreateAsync(user, model.Password);
			result.ThrowIfUnsuccessful();

			string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			string confirmUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);

			await _emailService.SendConfirmationUrlAsync(model.Email, confirmUrl);

			return Ok();
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> ConfirmEmail(long userId, string code)
		{
			return NotFound();
		}
	}
}
