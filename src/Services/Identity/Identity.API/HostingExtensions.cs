using Duende.IdentityServer;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Identity.API.Configs;
using Identity.API.Data;
using Identity.API.Interfaces;
using Identity.API.Models;
using Identity.API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Identity.API;

internal static class HostingExtensions
{
    private const string IdentityApiResourceName = "identity";
    private const string IdentityServiceDbConnection = "IdentityServiceDb";

    private static void InitializeDatabase(IApplicationBuilder app, IConfiguration configuration)
    {
        using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope();
        serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

        var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
        context.Database.Migrate();
        if (!context.Clients.Any())
        {
            foreach (var client in Config.GetClients(configuration))
            {
                context.Clients.Add(client.ToEntity());
            }
            context.SaveChanges();
        }

        if (!context.IdentityResources.Any())
        {
            foreach (var resource in Config.IdentityResources)
            {
                context.IdentityResources.Add(resource.ToEntity());
            }
            context.SaveChanges();
        }

        if (!context.ApiScopes.Any())
        {
            foreach (var apiScope in Config.ApiScopes)
            {
                context.ApiScopes.Add(apiScope.ToEntity());
            }
            context.SaveChanges();
        }
    }

    public static WebApplication ConfigureServices(this WebApplicationBuilder builder, IConfiguration configuration)
    {
        // Razor Pages & Controllers
        builder.Services.AddRazorPages();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        // Database
        var connectionString = configuration.GetConnectionString(IdentityServiceDbConnection)
            ?? throw new InvalidOperationException($"Connection string '{IdentityServiceDbConnection}' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Identity
        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // HttpContextAccessor & Services
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IUserAddressService, UserAddressService>();

        // IdentityServer
        builder.Services
            .AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.Endpoints.EnableCheckSessionEndpoint = true;
            })
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryApiResources(Config.ApiResources)
            .AddInMemoryClients(Config.GetClients(configuration))
            .AddAspNetIdentity<ApplicationUser>()
            .AddLicenseSummary();

        builder.Services.AddAuthentication()
            .AddLocalApi(options =>
            {
                options.ExpectedScope = "identity.fullaccess";
            });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireIdentityFullAccessScope", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "identity.fullaccess");
                policy.AuthenticationSchemes.Add(IdentityServerConstants.LocalApi.AuthenticationScheme);
            });
        });

        // External Auth (Google)
        var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
        var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        if (!string.IsNullOrWhiteSpace(googleClientId) && !string.IsNullOrWhiteSpace(googleClientSecret))
        {
            builder.Services
                .AddAuthentication()
                .AddGoogle(options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.ClientId = googleClientId;
                    options.ClientSecret = googleClientSecret;
                    options.CallbackPath = "/signin-google";
                });
        }

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        app.UseDeveloperExceptionPage();

        //InitializeDatabase(app, app.Configuration);

        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseIdentityServer();

        // Map API Controllers
        app.MapControllers();

        app.MapRazorPages()
            .RequireAuthorization();

        //app.UseHealthChecks("/health",
        //    new HealthCheckOptions
        //    {
        //        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        //    }
        //);

        return app;
    }
}
