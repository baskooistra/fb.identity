#pragma warning disable ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'

using Azure.Identity;
using CommunityToolkit.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Identity.Domain.Models;
using Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Identity.API.SeedData;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("IdentityContextConnection") ??
                       throw new InvalidOperationException("Connection string 'IdentityContextConnection' not found.");

builder.Services.AddDbContext<IdentityContext>(dbContextOptions =>
    dbContextOptions.UseSqlServer(connectionString, 
        sqlServerOptions =>
        {
            sqlServerOptions.MigrationsAssembly("Identity.API");
            sqlServerOptions.EnableRetryOnFailure();
        }));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<IdentityContext>();

builder.Services.AddRazorPages();

if (builder.Environment.IsDevelopment())    
    await SeedData.Initialize(builder.Services.BuildServiceProvider());
else
{
    builder.Configuration.AddAzureAppConfiguration(options =>
    {
        var endpoint = builder.Configuration.GetValue<string>("ConfigurationEndpoint");
        Guard.IsNotNullOrWhiteSpace(endpoint);
        options.Connect(new Uri(endpoint), new ManagedIdentityCredential());
    });
    
    builder.Services.AddApplicationInsightsTelemetry();   
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();