using Duende.IdentityServer.Models;
using Identity.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Threading;

namespace Identity.API.Areas.Oidc.Pages.Scopes
{
    public class IndexModel(IOidcScopesService scopesService) : PageModel
    {
        public DataTable ScopesTable { get; set; } = new();

        public async Task OnGet(CancellationToken cancellationToken)
        {
            ScopesTable = new DataTable();
            ScopesTable.Columns.Add("Name", typeof(string));
            ScopesTable.Columns.Add("DisplayName", typeof(string));
            ScopesTable.Columns.Add("Enabled", typeof(bool));

            var scopes = await scopesService.GetScopes(cancellationToken);
            foreach (var scope in scopes)
            {
                ScopesTable.Rows.Add(scope.Name, scope.DisplayName, scope.Enabled);
            }
        }

        public async Task OnPostDeleteAsync(string name, CancellationToken cancellationToken)
        {
            await scopesService.DeleteScope(new ApiScope(name), cancellationToken);
        }
    }
}
