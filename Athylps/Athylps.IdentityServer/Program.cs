using System.IO;
using System.Threading.Tasks;
using Athylps.IdentityServer.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Athylps.IdentityServer
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var host = new WebHostBuilder();
			var env = host.GetSetting("environment");
			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env}.json", optional: false)
				.AddEnvironmentVariables()
				.Build();

			Log.Logger = new LoggerConfiguration()
				.ReadFrom.Configuration(configuration)
				.Enrich.WithProperty("Environment", env)
				.CreateLogger();

			var builder = host.UseKestrel()
				.UseContentRoot(Directory.GetCurrentDirectory())
				.UseConfiguration(configuration)
				.UseSerilog()
				.UseUrls(configuration["Url"])
				.UseStartup<Startup>()
				.Build();

			await builder.InitializeIdentityServerDbAsync();
			await builder.InitializeAthylpsDbAsync();
			await builder.RunAsync();
		}
	}
}
