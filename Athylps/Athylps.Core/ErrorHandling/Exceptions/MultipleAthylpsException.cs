using System;
using System.Collections.Generic;
using System.Linq;
using Athylps.Core.ErrorHandling.Contracts;

namespace Athylps.Core.ErrorHandling.Exceptions
{
	public class MultipleAthylpsException : Exception
	{
		public IReadOnlyList<AthylpsException> Exceptions { get; }

		public MultipleAthylpsException(params ErrorCode[] errorCodes)
		{
			Exceptions = errorCodes
				.Select(ec => new AthylpsException(ec))
				.ToList();
		}

		public MultipleAthylpsException(params AthylpsException[] exceptions)
		{
			Exceptions = exceptions
				.Select(e => new AthylpsException(e.ErrorCode, e.Message))
				.ToList();
		}

		public virtual IReadOnlyList<ErrorContract> ToContracts()
		{
			return Exceptions.Select(e => e.ToContract()).ToList();
		}
	}
}
