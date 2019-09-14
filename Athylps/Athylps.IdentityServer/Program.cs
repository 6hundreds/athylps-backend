﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Athylps.IdentityServer.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Athylps.IdentityServer
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			bool seed = args.Any(x => x == "-seed");

			if (seed)
			{
				args = args.Except(new[] { "-seed" }).ToArray();
			}

			IWebHost host = CreateWebHost(args);

			if (seed)
			{
				await host.InitializeIdentityServerStorages();
			}

			await host.RunAsync();
		}


		private static IWebHost CreateWebHost(string[] args)
		{
			string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? EnvironmentName.Production;

			var configuration = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{environment}.json", optional: false)
				.AddEnvironmentVariables()
				.Build();

			Log.Logger = new LoggerConfiguration()
				.ReadFrom.Configuration(configuration)
				.Enrich.WithProperty("Application", "Athylps.IdentityServer")
				.Enrich.WithProperty("Environment", environment)
				.CreateLogger();

			return WebHost.CreateDefaultBuilder(args)
				.UseConfiguration(configuration)
				.UseKestrel()
				.UseUrls(configuration["Url"])
				.UseContentRoot(Directory.GetCurrentDirectory())
				.UseSerilog()
				.UseShutdownTimeout(TimeSpan.FromSeconds(10))
				.UseStartup<Startup>()
				.Build();
		}
	}
}
