﻿using System.Collections.Generic;

namespace Athylps.Core.ErrorHandling.Contracts
{
	public class ErrorContainer
	{
		public int Status { get; set; }
		public List<ErrorContract> Errors { get; }

		public ErrorContainer()
		{
			Errors = new List<ErrorContract>();
		}
	}
}
