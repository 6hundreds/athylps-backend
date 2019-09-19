using System;
using Athylps.Core.ErrorHandling.Contracts;

namespace Athylps.Core.ErrorHandling.Exceptions
{
	public class AthylpsException : Exception
	{
		public ErrorCode ErrorCode { get; }

		public AthylpsException(ErrorCode errorCode)
		{
			ErrorCode = errorCode;
		}

		public AthylpsException(ErrorCode errorCode, string message) : base(message)
		{
			ErrorCode = errorCode;
		}

		public virtual ErrorContract ToContract()
		{
			string message = string.IsNullOrWhiteSpace(Message)
				? ErrorMessages.Get(ErrorCode)
				: Message;

			return new ErrorContract
			{
				Code = ErrorCode,
				Message = message
			};
		}
	}
}
