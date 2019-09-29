using System;
using System.Collections.Generic;

namespace Athylps.Core.ErrorHandling.Exceptions
{
	public class AthylpsMultipleException : Exception
	{
		public IReadOnlyList<AthylpsException> Exceptions { get; }

		public AthylpsMultipleException(IEnumerable<AthylpsException> exceptions)
		{
			Exceptions = new List<AthylpsException>(exceptions);
		}
	}
}
