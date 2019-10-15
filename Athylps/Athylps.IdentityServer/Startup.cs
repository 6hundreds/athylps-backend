using System;
using System.Reflection;
using Athylps.Core.Entities;
using Athylps.IdentityServer.Models.Options;
using Athylps.IdentityServer.Validators;
using Athylps.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Athylps.IdentityServer
{
	public class Startup
	{
		public IConfiguration Configuration { get; }
		public IHostingEnvironment Environment { get; }

		public Startup(IConfiguration configuration, IHostingEnvironment environment)
		{
			Configuration = configuration;
			Environment = environment;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<IdentityServerOptions>(Configuration.GetSection("IdentityServer"));

			ConfigureAspCoreIdentity(services);
			ConfigureIdentityServer(services);

			services.AddDbContext<AthylpsDbContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString(Constants.AthylpsDbName)));

			services.AddMvc()
				.SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_2);
		}

		private void ConfigureAspCoreIdentity(IServiceCollection services)
		{
			services
				.AddIdentity<User, Role>(options =>
				{
					options.Password.RequireNonAlphanumeric = false;
				})
				.AddEntityFrameworkStores<AthylpsDbContext>()
				.AddDefaultTokenProviders();
		}

		private void ConfigureIdentityServer(IServiceCollection services)
		{
			string migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

			IIdentityServerBuilder builder = services.AddIdentityServer(options =>
				{
					options.Events.RaiseErrorEvents = true;
					options.Events.RaiseInformationEvents = true;
					options.Events.RaiseFailureEvents = true;
					options.Events.RaiseSuccessEvents = true;
				})
				.AddConfigurationStore(options =>
				{
					options.ConfigureDbContext = db =>
						db.UseSqlServer(Configuration.GetConnectionString(Constants.ConfigurationDbName),
							sql => sql.MigrationsAssembly(migrationsAssembly));
				})
				.AddOperationalStore(options =>
				{
					options.ConfigureDbContext = db =>
						db.UseSqlServer(Configuration.GetConnectionString(Constants.PersistedGrantDbName),
							sql => sql.MigrationsAssembly(migrationsAssembly));

					options.EnableTokenCleanup = true;
				})
				.AddAspNetIdentity<User>()
				.AddResourceOwnerValidator<EmailAndUsernameResourceOwnerPasswordValidator>();

			if (Environment.IsDevelopment())
			{
				builder.AddDeveloperSigningCredential();
			}
			else
			{
				throw new Exception("need to configure key material");
			}
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (Environment.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseStaticFiles();
			app.UseIdentityServer();
			app.UseMvcWithDefaultRoute();
		}
	}
}
