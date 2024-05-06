#pragma warning disable ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'

using Identity.API.Extensions;
using Identity.API.SeedData;
using Identity.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var development = builder.Environment.IsDevelopment();

if (!development)
    builder.AddCloudHostedServices();

builder
    .AddLogging()
    .AddDatabase()
    .AddIdentity()
    .AddIdentityServer()
    .AddAspNetEndpoints()
    .AddCors();

builder.Services.AddInfrastructure();

if (development)
{
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
}

await SeedData.Initialize(builder.Services.BuildServiceProvider(), builder.Environment.IsDevelopment());

var app = builder.Build();

app.ConfigurePipeline();

app.Run();