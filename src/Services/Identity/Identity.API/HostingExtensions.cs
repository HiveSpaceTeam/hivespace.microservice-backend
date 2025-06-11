using Duende.IdentityServer;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using HealthChecks.UI.Client;
using Identity.API.Data;
using Identity.API.Models;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Identity.API;

internal static class HostingExtensions
{
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
        builder.Services.AddRazorPages();
        var migrationsAssembly = typeof(Program).Assembly.GetName().Name;
        var connectionString = configuration.GetConnectionString("IdentityServiceDb");

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
        })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        builder.Services
            .AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.Endpoints.EnableCheckSessionEndpoint = true;
            })
            //.AddConfigurationStore(options =>
            //{
            //    options.ConfigureDbContext = db =>
            //        db.UseSqlServer(connectionString);
            //})
            //.AddOperationalStore(options =>
            //{

            //   options.ConfigureDbContext = db =>
            //        db.UseSqlServer(connectionString);
            //})
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryClients(Config.GetClients(configuration))
            .AddAspNetIdentity<ApplicationUser>()
            .AddLicenseSummary();

        var authenticationBuilder = builder.Services.AddAuthentication();

        var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
        var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

        if (!string.IsNullOrWhiteSpace(googleClientId) && !string.IsNullOrWhiteSpace(googleClientSecret))
        {
            authenticationBuilder
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

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        //InitializeDatabase(app, app.Configuration);

        app.UseStaticFiles();
        app.UseRouting();
        app.UseIdentityServer();
        app.UseAuthorization();

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
