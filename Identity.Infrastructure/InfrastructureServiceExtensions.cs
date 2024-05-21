using Identity.Domain.Interfaces;
using Identity.Infrastructure.Events;
using Identity.Infrastructure.Repositories;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddTransient<IOidcClientsService, OidcClientsRepository>()
            .AddTransient<IOidcScopesService, OidcScopesRepository>()
            .AddEventGrid(configuration);
    }
}