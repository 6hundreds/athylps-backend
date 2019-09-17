using System.Threading.Tasks;
using Athylps.Core.Entities;
using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Athylps.IdentityServer.Validators
{
	public class EmailAndUsernameResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
	{
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly IEventService _eventService;

		public EmailAndUsernameResourceOwnerPasswordValidator(UserManager<User> userManager, 
			SignInManager<User> signInManager,
			IEventService eventService)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_eventService = eventService;
		}

		public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
		{
			string userName = _userManager.NormalizeKey(context.UserName);

			User user = await _userManager.Users
				.FirstOrDefaultAsync(u => u.NormalizedUserName == userName || 
				                          u.NormalizedEmail == userName);

			if (user != null)
			{
				await ProcessSignInAsync(user, context);
			}
			else
			{
				await ProcessSignInErrorAsync(null, context);
			}
		}

		private async Task ProcessSignInAsync(User user, ResourceOwnerPasswordValidationContext context)
		{
			SignInResult signInResult = await _signInManager.CheckPasswordSignInAsync(user, 
				context.Password, 
				false
			);

			if (signInResult.Succeeded)
			{
				string sub = user.Id.ToString();

				UserLoginSuccessEvent identityEvent = new UserLoginSuccessEvent(user.UserName,
					sub,
					context.UserName,
					false
				);

				await SetValidationResultAsync("Credentials validated for username: {username}",
					context,
					identityEvent,
					new GrantValidationResult(sub, OidcConstants.AuthenticationMethods.Password)
				);
			}
			else
			{
				await ProcessSignInErrorAsync(signInResult, context);
			}
		}

		private async Task ProcessSignInErrorAsync(SignInResult signInResult, ResourceOwnerPasswordValidationContext context)
		{
			string logMessage;
			string eventError;

			if (signInResult == null)
			{
				logMessage = "No user found matching username or email: {username}";
				eventError = "Invalid username";
			}
			else if (signInResult.IsLockedOut)
			{
				logMessage = "Authentication failed for username: {username}, reason: locked out";
				eventError = "Locked out";
			}
			else if (signInResult.IsNotAllowed)
			{
				logMessage = "Authentication failed for username: {username}, reason: not allowed";
				eventError = "Not allowed";
			}
			else
			{
				eventError = "Invalid credentials";
				logMessage = "Authentication failed for username: {username}, reason: invalid credentials";
			}

			UserLoginFailureEvent identityEvent = new UserLoginFailureEvent(context.UserName,
				eventError,
				false
			);

			await SetValidationResultAsync(logMessage,
				context,
				identityEvent,
				new GrantValidationResult(TokenRequestErrors.InvalidGrant)
			);
		}

		private async Task SetValidationResultAsync(string logMessage, 
			ResourceOwnerPasswordValidationContext context,
			Event identityEvent, 
			GrantValidationResult result)
		{
			Log.Information(logMessage, context.UserName);

			await _eventService.RaiseAsync(identityEvent);

			context.Result = result;
		}
	}
}
