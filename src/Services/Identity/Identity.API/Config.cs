using Duende.IdentityServer;
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

    public static IEnumerable<Client> GetClients(IConfiguration configuration) =>
        [
            new Client
            {
                ClientId = "webapp",
                ClientName = "WebApp Client",
                ClientSecrets = 
                {
                    new Secret("secret".Sha256())
                },
                ClientUri = $"{configuration["WebAppClient"]}",                             // public uri of the client
                AllowedGrantTypes = GrantTypes.Code,
                AllowAccessTokensViaBrowser = false,
                RequireConsent = false,
                AllowOfflineAccess = true,
                AlwaysIncludeUserClaimsInIdToken = true,
                RequirePkce = false,
                RedirectUris = 
                {
                    $"{configuration["WebAppClient"]}/signin-oidc"
                },
                PostLogoutRedirectUris =
                [
                    $"{configuration["WebAppClient"]}/signout-callback-oidc"
                ],
                AllowedScopes =
                [
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.OfflineAccess,
                    "order",
                    "basket",
                    "catalog"
                ],
                AccessTokenLifetime = 60*60*2, // 2 hours
                IdentityTokenLifetime= 60*60*2 // 2 hours
            },

            // interactive client using code flow + pkce
            //new Client
            //{
            //    ClientId = "interactive",
            //    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

            //    AllowedGrantTypes = GrantTypes.Code,

            //    RedirectUris = { "https://localhost:44300/signin-oidc" },
            //    FrontChannelLogoutUri = "https://localhost:44300/signout-oidc",
            //    PostLogoutRedirectUris = { "https://localhost:44300/signout-callback-oidc" },

            //    AllowOfflineAccess = true,
            //    AllowedScopes = { "openid", "profile", "scope2" }
            //},
        ];
}
