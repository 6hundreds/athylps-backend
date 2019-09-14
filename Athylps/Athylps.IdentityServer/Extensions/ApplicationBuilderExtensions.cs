using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Athylps.Core.Data.Context;
using Athylps.Core.Entities;
using Athylps.Core.Types;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Athylps.IdentityServer.Extensions
{
	public static class ApplicationBuilderExtensions
	{
		public static async Task InitializeIdentityServerStorages(this IWebHost webHost)
		{
			using (IServiceScope serviceScope = webHost.Services.CreateScope())
			{
				await InitializePersistedGrantStorage(serviceScope);
				await InitializeConfigurationStorage(serviceScope);
				await InitializeAthylpsStorage(serviceScope);
			}
		}

		private static async Task InitializePersistedGrantStorage(IServiceScope serviceScope)
		{
			PersistedGrantDbContext grantDbContext = serviceScope.ServiceProvider.GetService<PersistedGrantDbContext>();
			await grantDbContext.Database.EnsureCreatedAsync();
		}

		private static async Task InitializeConfigurationStorage(IServiceScope serviceScope)
		{
			ConfigurationDbContext configurationDbContext = serviceScope.ServiceProvider.GetService<ConfigurationDbContext>();
			await configurationDbContext.Database.EnsureCreatedAsync();

			if (!await configurationDbContext.Clients.AnyAsync())
			{
				IConfiguration configuration = serviceScope.ServiceProvider.GetRequiredService<IConfiguration>();
					
				foreach (Client client in Configuration.GetClients(configuration))
				{
					await configurationDbContext.Clients.AddAsync(client.ToEntity());
				}
			}

			if (!await configurationDbContext.IdentityResources.AnyAsync())
			{
				foreach (IdentityResource resource in Configuration.GetIdentityResources())
				{
					await configurationDbContext.IdentityResources.AddAsync(resource.ToEntity());
				}
			}

			if (!await configurationDbContext.ApiResources.AnyAsync())
			{
				foreach (ApiResource resource in Configuration.GetApis())
				{
					await configurationDbContext.ApiResources.AddAsync(resource.ToEntity());
				}
			}

			await configurationDbContext.SaveChangesAsync();
		}

		private static async Task InitializeAthylpsStorage(IServiceScope serviceScope)
		{
			AthylpsDbContext athylpsDbContext = serviceScope.ServiceProvider.GetService<AthylpsDbContext>();
			await athylpsDbContext.Database.EnsureCreatedAsync();

			var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<Role>>();

			foreach (string roleName in Roles.ToArray())
			{
				Role role = await roleManager.FindByNameAsync(roleName);
				
				if (role == null)
				{
					role = new Role(roleName);
					await roleManager.CreateAsync(role);
				}
			}

			var userManager = serviceScope.ServiceProvider.GetService<UserManager<User>>();

			User admin = await userManager.FindByNameAsync("admin");
			
			if (admin == null)
			{
				admin = new User
				{
					UserName = "admin",
					EmailConfirmed = true,
					Email = "a@email.com"
				};

				IConfiguration configuration = serviceScope.ServiceProvider.GetService<IConfiguration>();
				IdentityResult result = await userManager.CreateAsync(admin, configuration["Admin:Password"]);

				if (!result.Succeeded)
				{
					IdentityError error = result.Errors.FirstOrDefault();

					Log.Error("Ошибка при создании администратора: {error}", error?.Description);
					throw new Exception(error?.Description);
				}

				admin = await userManager.FindByNameAsync("admin");
				
				result = await userManager.AddClaimsAsync(admin, new[]
				{
					new Claim(JwtClaimTypes.Name, "Administrator")
				});

				if (!result.Succeeded)
				{
					IdentityError error = result.Errors.FirstOrDefault();

					Log.Error("Ошибка при добавлении claim: {error}", error?.Description);
					throw new Exception(error?.Description);
				}
			}
		}
	}
}
