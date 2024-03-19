#pragma warning disable ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'

using Azure.Identity;
using CommunityToolkit.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Identity.Domain.Models;
using Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Identity.API.SeedData;
using Serilog;
using Identity.API.Extensions;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(Serilog.Events.LogEventLevel.Debug)
    .MinimumLevel.Debug()
    .CreateBootstrapLogger();

Log.Information("Starting identity server...");

try
{
    var builder = WebApplication.CreateBuilder(args);
    var development = builder.Environment.IsDevelopment();

    if (!development)
        builder.AddCloudHostedServices();

    builder
        .AddLogging()
        .AddDatabase()
        .AddIdentity()
        .AddAspNetEndpoints();

    if (development)
        await SeedData.Initialize(builder.Services.BuildServiceProvider());

    var app = builder.Build();

    app.ConfigurePipeline();

    app.Run();
}
catch (Exception exc)
{
    Log.Fatal(exc, "Unable to start identity server");
}
finally
{
    Log.Information("Identity server termimnated");
    Log.CloseAndFlush();
}