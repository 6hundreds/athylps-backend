using System.Collections.Generic;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.Extensions.Configuration;

namespace Athylps.IdentityServer
{
	public static class Config
	{
		public static IEnumerable<IdentityResource> GetIdentityResources()
		{
			return new IdentityResource[]
			{
				new IdentityResources.OpenId(),
				new IdentityResources.Profile()
			};
		}

		public static IEnumerable<ApiResource> GetApis()
		{
			return new[]
			{
				new ApiResource("api1", "Athylps API #1")
			};
		}

		public static IEnumerable<Client> GetClients(IConfiguration configuration)
		{
			IConfigurationSection clientsSettings = configuration.GetSection("IdentityServerSettings:Clients");
			
			return new[]
			{
				new Client
				{
					ClientId = clientsSettings["Api1:ClientId"],
					ClientName = clientsSettings["Api1:ClientName"],
					ClientSecrets = { new Secret(clientsSettings["Api1:ClientSecret"].Sha256()) },
					
					AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
					AllowedScopes = { "api1" },
					AllowOfflineAccess = true
				}
			};
		}
		public static List<TestUser> GetTestUsers()
		{
			return new List<TestUser>
			{
				new TestUser
				{
					SubjectId = "1",
					Username = "alice",
					Password = "password"
				},
				new TestUser
				{
					SubjectId = "2",
					Username = "bob",
					Password = "password"
				}
			};
		}
	}
}
