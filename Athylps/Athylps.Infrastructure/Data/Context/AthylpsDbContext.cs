using Athylps.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Athylps.Infrastructure.Data.Context
{
	public class AthylpsDbContext : IdentityDbContext<User, Role, long>
	{
		public AthylpsDbContext(DbContextOptions<AthylpsDbContext> options) : base(options)
		{
		}
	}
}
