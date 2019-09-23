using System;
using System.Net;
using System.Threading.Tasks;
using Athylps.Core.ErrorHandling.Contracts;
using Athylps.Core.ErrorHandling.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Athylps.Core.ErrorHandling.Middleware
{
	public class ExceptionHandlingMiddleware
	{
		private readonly RequestDelegate _next;

		public ExceptionHandlingMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext httpContext)
		{
			try
			{
				await _next(httpContext);
			}
			catch (Exception e)
			{
				await HandleExceptionAsync(e, httpContext);
			}
		}

		private async Task HandleExceptionAsync(Exception exception, HttpContext context)
		{
			ErrorContainer errorContainer = new ErrorContainer();

			switch (exception)
			{
				case AthylpsException ae:
					errorContainer.Status = (int)HttpStatusCode.BadRequest;
					errorContainer.Errors.Add(ae.ToContract());
					break;

				default:
					errorContainer.Status = (int)HttpStatusCode.InternalServerError;
					errorContainer.Errors.Add(new ErrorContract
					{
						Message = exception.Message,
						Code = ErrorCode.UnspecifiedError
					});
					break;
			}

			context.Response.StatusCode = errorContainer.Status;
			context.Response.ContentType = "application/json";
			string result = JsonConvert.SerializeObject(errorContainer);

			await context.Response.WriteAsync(result);
		}
	}

	public static class ExceptionHandlingMiddlewareExtensions
	{
		public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<ExceptionHandlingMiddleware>();
		}
	}
}
