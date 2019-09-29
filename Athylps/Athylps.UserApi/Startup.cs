using Athylps.Core.Data.Context;
using Athylps.Core.Entities;
using Athylps.Core.ErrorHandling.Middleware;
using Athylps.UserApi.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Athylps.UserApi
{
	public class Startup
	{
		private readonly IConfiguration _configuration;

		public Startup(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<AthylpsDbContext>(options =>
				options.UseSqlServer(_configuration.GetConnectionString("AthylpsDb")));

			services.AddAuthentication("Bearer")
				.AddJwtBearer("Bearer", options =>
				{
					options.Audience = _configuration["IdentityServer:Audience"];
					options.Authority = _configuration["IdentityServer:Authority"];
					options.RequireHttpsMetadata = false;
				});

			services.AddIdentity<User, Role>()
				.AddEntityFrameworkStores<AthylpsDbContext>();

			services.AddMvc(options =>
				{
					options.Filters.Add<ValidateModelAttribute>();
				})
				.AddJsonOptions(options =>
					options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore)
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

			services.Configure<ApiBehaviorOptions>(options =>
			{
				options.SuppressModelStateInvalidFilter = true;
			});
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			app.UseAuthentication();
			app.UseExceptionHandlingMiddleware();
			app.UseMvc();
		}
	}
}
