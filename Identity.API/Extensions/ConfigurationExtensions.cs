using AspNetCore.Authentication.ApiKey;
using Azure.Identity;
using CommunityToolkit.Diagnostics;
using Identity.API.Authentication;
using Identity.API.Constants;
using Identity.Domain.Models;
using Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;

namespace Identity.API.Extensions;

public static class ConfigurationExtensions
{
    const string AppConfigurationEndpoint = "ConfigurationEndpoint";
    const string AppConfigurationKey = "ConfigurationKey";

    public static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.Console(LogEventLevel.Debug, outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
        .WriteTo.ApplicationInsights(TelemetryConverter.Events, LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(ctx.Configuration));

        return builder;
    }

    public static WebApplicationBuilder AddCloudHostedServices(this WebApplicationBuilder builder)
    {
        builder.Configuration.AddAzureAppConfiguration(options =>
        {
            var endpoint = builder.Configuration.GetValue<string>(AppConfigurationEndpoint);
            var configurationKey = builder.Configuration.GetValue<string>(AppConfigurationKey) + ":";
            var environmentName = builder.Environment.EnvironmentName;

            Guard.IsNotNullOrWhiteSpace(endpoint);
            var config = options.Connect(new Uri(endpoint), new ManagedIdentityCredential())
                .ConfigureKeyVault(kv => kv.SetCredential(new ManagedIdentityCredential()))
                .Select($"{configurationKey}*", environmentName)
                .TrimKeyPrefix(configurationKey);
        });

        builder.Services.AddApplicationInsightsTelemetry();

        return builder;
    }

    public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
    {
        // Add services to the container.
        var connectionString = builder.Configuration.GetConnectionString("IdentityContextConnection") ??
                               throw new InvalidOperationException("Connection string 'IdentityContextConnection' not found.");

        builder.Services.AddDbContext<IdentityContext>(dbContextOptions =>
            dbContextOptions.UseSqlServer(connectionString,
                sqlServerOptions =>
                {
                    sqlServerOptions.MigrationsAssembly(typeof(Program).Assembly.FullName);
                    sqlServerOptions.EnableRetryOnFailure();
                }));

        if (builder.Environment.IsDevelopment())
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        return builder;
    }

    public static WebApplicationBuilder AddIdentity(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddDefaultIdentity<User>(options => 
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.User.RequireUniqueEmail = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<IdentityContext>();

        return builder;
    }

    public static WebApplicationBuilder AddAspNetEndpoints(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<AuthenticationConfiguration>(builder.Configuration.GetSection(nameof(AuthenticationConfiguration)));

        builder.Services.AddAuthentication()
            .AddApiKeyInHeader<ApiKeyProvider>(options =>
            {
                options.Realm = "Azure functions";
                options.KeyName = "x-api-key";
            });

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(AuthorizationPolicies.Owner, policy =>
                policy.RequireRole(Roles.Owner));

        builder.Services.AddRazorPages(options =>
        {
            options.Conventions.AuthorizeAreaFolder("oidc", "/");
            options.Conventions.AuthorizeAreaPage("identity", "/users", AuthorizationPolicies.Owner);
        });

        builder.Services.AddControllers();

        return builder;
    }

    public static WebApplicationBuilder AddIdentityServer(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("IdentityContextConnection") ??
                               throw new InvalidOperationException("Connection string 'IdentityContextConnection' not found.");

        builder.Services.AddIdentityServer()
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = config =>
                    config.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(typeof(Program).Assembly.GetName().Name));
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = config =>
                    config.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(typeof(Program).Assembly.GetName().Name));
            })
            .AddAspNetIdentity<User>();

        return builder;
    }

    public static WebApplicationBuilder AddCors(this WebApplicationBuilder builder)
    {
        var allowedDomains = builder.Configuration.GetSection("AllowedDomains").Get<string[]>();
        if (allowedDomains?.Length > 0)
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy
                        .WithOrigins(allowedDomains!)
                        .AllowCredentials()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

        return builder;
    }
}