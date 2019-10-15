using System.Collections.Generic;

namespace Athylps.IdentityServer.Models.Options
{
	internal class IdentityServerOptions
	{
		public ICollection<ClientOptoins> Clients { get; set; }
		public ICollection<UserOptions> Users { get; set; }

		public IdentityServerOptions()
		{
			Clients = new HashSet<ClientOptoins>();
			Users = new HashSet<UserOptions>();
		}
	}
}
