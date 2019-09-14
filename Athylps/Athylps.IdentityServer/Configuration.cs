using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;

namespace Athylps.IdentityServer
{
	public static class Configuration
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
				new ApiResource("userapi", "Athylps User API")
			};
		}

		public static IEnumerable<Client> GetClients(IConfiguration configuration)
		{
			IConfigurationSection clientsSettings = configuration.GetSection("IdentityServer:Clients");

			return new[]
			{
				new Client
				{
					ClientId = clientsSettings["ResourceOwner:ClientId"],
					ClientName = clientsSettings["ResourceOwner:Name"],
					AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
					AllowOfflineAccess = true,

					ClientSecrets =
					{
						new Secret(clientsSettings["ResourceOwner:Secret"].Sha256())
					},

					AllowedScopes =
					{
						IdentityServerConstants.StandardScopes.OpenId,
						IdentityServerConstants.StandardScopes.Profile,
						"userapi"
					}
				},
			};
		}
	}
}
