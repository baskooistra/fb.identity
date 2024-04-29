using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Models;
using Identity.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Repositories;

public class OidcClientsRepository(
    ConfigurationDbContext context) : IOidcClientsService
{
    async Task<IEnumerable<Client>> IOidcClientsService.GetClients(CancellationToken cancellationToken)
    {
        return await context.Clients
            .Select(client => client.ToModel())
            .ToListAsync(cancellationToken);
    }

    async Task<Client?> IOidcClientsService.FindClient(string clientId, CancellationToken cancellationToken)
    {
        var clientEntity = await context.Clients.FindAsync([clientId], cancellationToken: cancellationToken);
        return clientEntity?.ToModel();
    }

    async Task<Client> IOidcClientsService.AddClient(Client client, CancellationToken cancellationToken)
    {
        var clientEntity = context.Clients.Add(client.ToEntity());
        await context.SaveChangesAsync(cancellationToken);
        return clientEntity.Entity.ToModel();
    }

    async Task<Client> IOidcClientsService.UpdateClient(Client client, CancellationToken cancellationToken)
    {
        var clientEntity = await context.Clients.FindAsync([client.ClientId], cancellationToken: cancellationToken);
        if (clientEntity == null)
            throw new KeyNotFoundException("No client found matching client id");

        var updateEntity = client.ToEntity();
        clientEntity.ClientName = updateEntity.ClientName;
        clientEntity.Description = updateEntity.Description;
        clientEntity.Enabled = updateEntity.Enabled;
        clientEntity.AllowedGrantTypes = updateEntity.AllowedGrantTypes;
        clientEntity.RequirePkce = updateEntity.RequirePkce;
        clientEntity.AllowOfflineAccess = updateEntity.AllowOfflineAccess;
        clientEntity.RedirectUris = updateEntity.RedirectUris;
        clientEntity.PostLogoutRedirectUris = updateEntity.PostLogoutRedirectUris;

        context.Entry(clientEntity).State = EntityState.Modified;
        await context.SaveChangesAsync(cancellationToken);

        return clientEntity.ToModel();
    }

    async Task IOidcClientsService.DeleteClient(string clientId, CancellationToken cancellationToken)
    {
        var clientEntity = await context.Clients.FindAsync([clientId], cancellationToken: cancellationToken);
        if (clientEntity == null)
            throw new KeyNotFoundException("No client found matching client id");
        
        context.Entry(clientEntity).State = EntityState.Deleted;
        await context.SaveChangesAsync(cancellationToken);
    }
}