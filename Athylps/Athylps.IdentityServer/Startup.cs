using System;
using System.Reflection;
using Athylps.Core.Entities;
using Athylps.Infrastructure.Data.Context;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
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
			services.AddDbContext<AthylpsDbContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("AthylpsDb")));

			services.AddIdentity<User, Role>()
				.AddEntityFrameworkStores<AthylpsDbContext>()
				.AddDefaultTokenProviders();

			services.AddMvc()
				.SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_2);

			string migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

			var builder = services.AddIdentityServer(options =>
				{
					options.Events.RaiseErrorEvents = true;
					options.Events.RaiseInformationEvents = true;
					options.Events.RaiseFailureEvents = true;
					options.Events.RaiseSuccessEvents = true;
				})
				.AddInMemoryIdentityResources(Config.GetIdentityResources())
				.AddInMemoryApiResources(Config.GetApis())
				.AddInMemoryClients(Config.GetClients(Configuration))
				.AddAspNetIdentity<User>()
				.AddConfigurationStore(options =>
				{
					options.ConfigureDbContext = db =>
						db.UseSqlServer(Configuration.GetConnectionString("ConfigurationDb"),
							sql => sql.MigrationsAssembly(migrationsAssembly));
				})
				.AddOperationalStore(options =>
				{
					options.ConfigureDbContext = db => 
						db.UseSqlServer(Configuration.GetConnectionString("OperationDb"),
							sql => sql.MigrationsAssembly(migrationsAssembly));

					options.EnableTokenCleanup = true;
				});

			if (Environment.IsDevelopment())
			{
				builder.AddDeveloperSigningCredential()
					.AddTestUsers(Config.GetTestUsers());
			}
			else
			{
				throw new Exception("need to configure key material");
			}
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			InitializeDatabase(app);

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
			}

			app.UseStaticFiles();
			app.UseIdentityServer();
			app.UseMvc();
		}

		private void InitializeDatabase(IApplicationBuilder app)
		{
			using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
			{
				serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

				var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
				context.Database.Migrate();

				if (!context.Clients.Any())
				{
					foreach (var client in Config.GetClients(Configuration))
					{
						context.Clients.Add(client.ToEntity());
					}
				}

				if (!context.IdentityResources.Any())
				{
					foreach (var resource in Config.GetIdentityResources())
					{
						context.IdentityResources.Add(resource.ToEntity());
					}
				}

				if (!context.ApiResources.Any())
				{
					foreach (var resource in Config.GetApis())
					{
						context.ApiResources.Add(resource.ToEntity());
					}
				}

				context.SaveChanges();
			}
		}
	}
}
