using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static Identity.API.Areas.Oidc.Pages.Clients.AddClient;
using System.ComponentModel.DataAnnotations;
using Identity.Domain.Interfaces;
using Azure.Security.KeyVault.Secrets;
using Duende.IdentityServer.Models;

namespace Identity.API.Areas.Oidc.Pages.Scopes
{
    public class AddScope(IOidcScopesService scopesService) : PageModel
    {
        public class ScopeModel()
        {
            public bool Enabled { get; set; }

            [Required(ErrorMessage = "This field is mandatory")]
            public required string Name { get; set; }

            public string? DisplayName { get; set; }
        }

        [BindProperty]
        public ScopeModel Input { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken) 
        {
            if (ModelState.IsValid)
            {
                var scope = new ApiScope(Input.Name);
                scope.Enabled = Input.Enabled;
                scope.DisplayName = Input.DisplayName;

               await scopesService.AddScope(scope, cancellationToken);

                return RedirectToPage("oidc/scopes");
            }

            return Page();
        }
    }
}
