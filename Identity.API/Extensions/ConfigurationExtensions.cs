using Azure.Identity;
using CommunityToolkit.Diagnostics;
using Identity.Domain.Models;
using Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        builder.Services.AddRazorPages();

        return builder;
    }
}