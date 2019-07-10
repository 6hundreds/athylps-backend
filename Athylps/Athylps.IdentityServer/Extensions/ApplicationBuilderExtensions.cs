using System;
using System.Linq;
using System.Threading.Tasks;
using Athylps.Core.Entities;
using Athylps.IdentityServer.Types;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
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
		public static async Task InitializeIdentityServerDbAsync(this IWebHost webHost)
		{
			using (var serviceScope = webHost.Services.GetService<IServiceScopeFactory>().CreateScope())
			{
				await serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.MigrateAsync();

				var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
				await context.Database.MigrateAsync();

				if (!await context.Clients.AnyAsync())
				{
					var configuration = serviceScope.ServiceProvider.GetRequiredService<IConfiguration>();

					foreach (var client in Config.GetClients(configuration))
					{
						context.Clients.Add(client.ToEntity());
					}
				}

				if (!await context.IdentityResources.AnyAsync())
				{
					foreach (var resource in Config.GetIdentityResources())
					{
						context.IdentityResources.Add(resource.ToEntity());
					}
				}

				if (!await context.ApiResources.AnyAsync())
				{
					foreach (var resource in Config.GetApis())
					{
						context.ApiResources.Add(resource.ToEntity());
					}
				}

				await context.SaveChangesAsync();
			}
		}

		public static async Task InitializeAthylpsDbAsync(this IWebHost webHost)
		{
			using (var serviceScope = webHost.Services.GetService<IServiceScopeFactory>().CreateScope())
			{
				var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

				foreach (string roleName in Roles.ToArray())
				{
					Role role = await roleManager.FindByNameAsync(roleName);
					
					if (role == null)
					{
						role = new Role(roleName);
						await roleManager.CreateAsync(role);
					}
				}

				var adminSettings = serviceScope.ServiceProvider.GetRequiredService<IConfiguration>()
					.GetSection("IdentityServerSettings:Admin");

				var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
				var email = adminSettings["Email"];

				var admin = await userManager.FindByEmailAsync(email);
				if (admin == null)
				{
					admin = new User
					{
						Email = email, 
						UserName = adminSettings["Name"]
					};

					var password = adminSettings["Password"];
					IdentityResult result = await userManager.CreateAsync(admin, password);

					if (!result.Succeeded)
					{
						var errors = string.Join("\n", result.Errors.Select(e => $"{e.Code}: {e.Description}").ToList());
						var message = $"Ошибка при инициализации админа. {errors}";
						
						Log.Error(message);
						throw new Exception(message);
					}

					await userManager.AddToRoleAsync(admin, Roles.Admin);
				}
			}
		}
	}
}
