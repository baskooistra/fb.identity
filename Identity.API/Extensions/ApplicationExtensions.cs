namespace Identity.API.Extensions;

public static class ApplicationExtensions
{
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSecurityHeaders();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage()
                .UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error")
                .UseHsts();
        }

        app.UseHttpsRedirection()
            .UseStaticFiles()
            .UseRouting()
            .UseCors()
            .UseIdentityServer()
            .UseAuthorization();
            
        app.MapControllers();
        app.MapRazorPages();

        return app;
    }

    private static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
    {
        app.UseSecurityHeaders(policies =>
            policies.AddDefaultSecurityHeaders()
                .AddContentSecurityPolicy(builder =>
                {
                    builder.AddDefaultSrc().Self();
                    builder.AddObjectSrc().None();
                    builder.AddFrameAncestors().None();
                    builder.AddSandbox().AllowForms().AllowSameOrigin().AllowScripts();
                    builder.AddBaseUri().Self();
                    builder.AddUpgradeInsecureRequests();
                }));

        return app;
    }
}
