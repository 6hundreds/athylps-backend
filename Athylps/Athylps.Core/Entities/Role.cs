using Microsoft.AspNetCore.Identity;

namespace Athylps.Core.Entities
{
	public class Role : IdentityRole<long>
	{
		public Role()
		{
		}

		public Role(string roleName) : base(roleName)
		{
		}
	}
}
