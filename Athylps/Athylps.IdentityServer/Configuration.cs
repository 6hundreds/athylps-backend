using System.Collections.Generic;
using System.Linq;
using Athylps.IdentityServer.Models.Options;
using IdentityServer4;
using IdentityServer4.Models;

namespace Athylps.IdentityServer
{
	internal static class Configuration
	{
		internal static IEnumerable<IdentityResource> GetIdentityResources()
		{
			return new IdentityResource[]
			{
				new IdentityResources.OpenId(),
				new IdentityResources.Profile()
			};
		}

		internal static IEnumerable<ApiResource> GetApis()
		{
			return new[]
			{
				new ApiResource("userapi", "Athylps User API")
			};
		}

		internal static IEnumerable<Client> GetClients(ICollection<ClientOptoins> clients)
		{
			List<Client> identityClients = new List<Client>();

			Dictionary<string, ClientOptoins> optionsData = clients.ToDictionary(c => c.Type, c => c);

			if (optionsData.TryGetValue("ResourceOwner", out ClientOptoins options))
			{
				identityClients.Add(new Client
				{
					ClientId = options.ClientId,
					ClientName = options.Name,
					AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
					AllowOfflineAccess = true,
					ClientSecrets =
					{
						new Secret(options.Secret.Sha256())
					},
					AllowedScopes =
					{
						IdentityServerConstants.StandardScopes.OpenId,
						IdentityServerConstants.StandardScopes.Profile,
						"userapi"
					}
				});
			}
			
			return identityClients;
		}
	}
}
