using Duende.IdentityServer.Models;

namespace Identity.API;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        [
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        ];

    public static IEnumerable<ApiScope> ApiScopes =>
        [
            new ApiScope("order", "Order Service"),
            new ApiScope("basket", "Basket Service"),
            new ApiScope("catalog", "Catalog Service"),
            new ApiScope("identity", "Identity API")
        ];

    public static IEnumerable<ApiResource> ApiResources =>
        [
            new ApiResource("identity", "Identity API")
            {
                Scopes = { "identity", "openid", "profile", "offline_access", "order", "basket", "catalog" }
            }
        ];

    public static IEnumerable<Client> GetClients(IConfiguration configuration)
    {
        var clients = new List<Client>();
        var clientsSection = configuration.GetSection("Identity:Clients");
        
        // WebApp Client (full config)
        var webappConfig = clientsSection.GetSection("webapp").Get<ClientConfig>();
        if (webappConfig != null)
        {
            var webappClient = new Client
            {
                ClientId = webappConfig.ClientId,
                ClientName = webappConfig.ClientName,
                ClientUri = webappConfig.ClientUri,
                ClientSecrets = !string.IsNullOrEmpty(webappConfig.ClientSecret) 
                    ? new List<Secret> { new Secret(webappConfig.ClientSecret.Sha256()) }
                    : new List<Secret>(),
                RequireClientSecret = webappConfig.RequireClientSecret,
                AllowedGrantTypes = webappConfig.AllowedGrantTypes ?? ["authorization_code"],
                AllowAccessTokensViaBrowser = webappConfig.AllowAccessTokensViaBrowser,
                RequireConsent = webappConfig.RequireConsent,
                AllowOfflineAccess = webappConfig.AllowOfflineAccess,
                AlwaysIncludeUserClaimsInIdToken = webappConfig.AlwaysIncludeUserClaimsInIdToken,
                RequirePkce = webappConfig.RequirePkce,
                RedirectUris = webappConfig.RedirectUris ?? [],
                PostLogoutRedirectUris = webappConfig.PostLogoutRedirectUris ?? [],
                AllowedCorsOrigins = webappConfig.AllowedCorsOrigins ?? [],
                AllowedScopes = webappConfig.AllowedScopes ?? [],
                AccessTokenLifetime = webappConfig.AccessTokenLifetime,
                IdentityTokenLifetime = webappConfig.IdentityTokenLifetime
            };
            clients.Add(webappClient);
        }

        // API Testing Client (minimal config)
        var apiTestingConfig = clientsSection.GetSection("apitestingapp").Get<ClientConfig>();
        if (apiTestingConfig != null)
        {
            var apiTestingClient = new Client
            {
                ClientId = apiTestingConfig.ClientId,
                ClientName = apiTestingConfig.ClientName,
                ClientSecrets = !string.IsNullOrEmpty(apiTestingConfig.ClientSecret)
                    ? new List<Secret> { new Secret(apiTestingConfig.ClientSecret.Sha256()) }
                    : new List<Secret>(),
                RequireClientSecret = apiTestingConfig.RequireClientSecret,
                AllowedGrantTypes = apiTestingConfig.AllowedGrantTypes ?? ["password"],
                AllowedScopes = apiTestingConfig.AllowedScopes ?? [],
                AlwaysIncludeUserClaimsInIdToken = apiTestingConfig.AlwaysIncludeUserClaimsInIdToken
            };
            clients.Add(apiTestingClient);
        }

        return clients;
    }
}
