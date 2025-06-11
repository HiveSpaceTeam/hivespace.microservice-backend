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
            new ApiScope("catalog", "Catalog Service")
        ];

    public static IEnumerable<Client> GetClients(IConfiguration configuration)
    {
        var clientsSection = configuration.GetSection("Identity:Clients");
        var clientConfigs = clientsSection.Get<List<ClientConfig>>() ?? [];
        return clientConfigs.Select(cfg => new Client
        {
            ClientId = cfg.ClientId,
            ClientName = cfg.ClientName,
            ClientUri = cfg.ClientUri,
            RequireClientSecret = cfg.RequireClientSecret,
            AllowedGrantTypes = cfg.AllowedGrantTypes ?? GrantTypes.Code,
            AllowAccessTokensViaBrowser = cfg.AllowAccessTokensViaBrowser,
            RequireConsent = cfg.RequireConsent,
            AllowOfflineAccess = cfg.AllowOfflineAccess,
            AlwaysIncludeUserClaimsInIdToken = cfg.AlwaysIncludeUserClaimsInIdToken,
            RequirePkce = cfg.RequirePkce,
            RedirectUris = cfg.RedirectUris ?? [],
            PostLogoutRedirectUris = cfg.PostLogoutRedirectUris ?? [],
            AllowedCorsOrigins = cfg.AllowedCorsOrigins ?? [],
            AllowedScopes = cfg.AllowedScopes ?? [],
            AccessTokenLifetime = cfg.AccessTokenLifetime,
            IdentityTokenLifetime = cfg.IdentityTokenLifetime
        });
    }
}
