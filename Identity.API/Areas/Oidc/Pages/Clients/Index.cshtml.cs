using Identity.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace Identity.API.Areas.Oidc.Pages.Clients
{
    public class IndexModel(IOidcClientsService oidcClientsService) : PageModel
    {
        public DataTable ClientsTable { get; set; } = new();

        public async Task OnGet(CancellationToken cancellationToken)
        {
            ClientsTable = new DataTable();
            ClientsTable.Columns.Add("ClientId", typeof(string));
            ClientsTable.Columns.Add("Description", typeof(string));
            ClientsTable.Columns.Add("Enabled", typeof(bool));

            var clients = await oidcClientsService.GetClients(cancellationToken);
            foreach (var client in clients)
            {
                ClientsTable.Rows.Add(client.ClientId, client.Description, client.Enabled);
            }
        }

        public async Task OnPostDeleteAsync(string clientId, CancellationToken cancellationToken)
        {
            await oidcClientsService.DeleteClient(clientId, cancellationToken);
        }
    }
}
