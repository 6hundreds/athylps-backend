using System.Collections.Generic;
using System.Linq;
using Athylps.Core.ErrorHandling.Contracts;
using Athylps.Core.ErrorHandling.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace Athylps.UserApi.Extensions
{
	internal static class IdentityResultExtensions
	{
		public static void ThrowIfUnsuccessful(this IdentityResult identityResult)
		{
			if (identityResult.Succeeded)
				return;

			List<AthylpsException> exceptions = identityResult.Errors
				.Select(e => new AthylpsException(ErrorCode.ModelValidationError, e.Description))
				.ToList();

			throw new AthylpsMultipleException(exceptions);
		}
	}
}
