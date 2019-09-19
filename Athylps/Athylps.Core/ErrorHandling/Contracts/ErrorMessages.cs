using System.Collections.Generic;

namespace Athylps.Core.ErrorHandling.Contracts
{
	public static class ErrorMessages
	{
		private static Dictionary<ErrorCode, string> _messages;

		static ErrorMessages()
		{
			_messages = new Dictionary<ErrorCode, string>
			{
				{ ErrorCode.UnspecifiedError, "Unexpected error" } 
			};
		}

		public static string Get(ErrorCode errorCode)
		{
			_messages.TryGetValue(errorCode, out string message);
			
			return message;
		}
	}
}
