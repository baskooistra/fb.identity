using Identity.Domain.Interfaces;
using Identity.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        return services.AddTransient<IOidcClientsService, OidcClientsRepository>()
            .AddTransient<IOidcScopesService, OidcScopesRepository>();
    }
}