using System.Collections.Generic;
using Athylps.Core.ErrorHandling.Contracts;

namespace Athylps.Core.ErrorHandling.Exceptions
{
	public class AthylpsMultipleException : AthylpsException
	{
		public IReadOnlyList<AthylpsException> Exceptions { get; }

		public AthylpsMultipleException(ErrorCode errorCode, IEnumerable<AthylpsException> exceptions) : base(errorCode)
		{
			Exceptions = new List<AthylpsException>(exceptions);
		}

		public AthylpsMultipleException(ErrorCode errorCode, string message, IEnumerable<AthylpsException> exceptions) : base(errorCode, message)
		{
			Exceptions = new List<AthylpsException>(exceptions);
		}
	}
}
