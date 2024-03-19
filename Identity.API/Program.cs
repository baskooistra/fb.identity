#pragma warning disable ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'

using Identity.API.Extensions;
using Identity.API.SeedData;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(LogEventLevel.Debug)
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