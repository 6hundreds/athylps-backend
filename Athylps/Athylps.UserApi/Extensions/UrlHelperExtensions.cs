using Athylps.UserApi.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Athylps.UserApi.Extensions
{
	internal static class UrlHelperExtensions
	{
		public static string EmailConfirmationLink(this IUrlHelper urlHelper, long userId, string code, string scheme)
		{
			return urlHelper.Action(
				nameof(AccountController.ConfirmEmail),
				"Account",
				new { userId, code },
				scheme
			);
		}
	}
}
