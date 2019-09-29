using System.Collections.Generic;
using System.Linq;
using Athylps.Core.ErrorHandling.Contracts;
using Athylps.Core.ErrorHandling.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Athylps.UserApi.Attributes
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid)
                return;

            List<AthylpsException> exceptions = context.ModelState.Values
                .Where(v => v.Errors.Any())
                .Select(v => v.Errors.First(e => !string.IsNullOrWhiteSpace(e.ErrorMessage)))
                .Select(e => new AthylpsException(ErrorCode.ModelValidationError, e.ErrorMessage))
                .ToList();

            throw new AthylpsMultipleException(ErrorCode.ModelValidationError, exceptions);
        }
    }
}
