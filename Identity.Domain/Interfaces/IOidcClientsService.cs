using Duende.IdentityServer.Models;

namespace Identity.Domain.Interfaces;

public interface IOidcClientsService
{
    Task<IEnumerable<Client>> GetClients(CancellationToken cancellationToken);
    Task<Client?> FindClient(string clientId, CancellationToken cancellationToken);
    Task<Client> AddClient(Client client, CancellationToken cancellationToken);
    Task<Client> UpdateClient(Client client, CancellationToken cancellationToken);
    Task DeleteClient(string clientId, CancellationToken cancellationToken);
}