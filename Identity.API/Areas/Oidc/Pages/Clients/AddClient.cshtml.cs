#nullable disable

using System.ComponentModel.DataAnnotations;
using Duende.IdentityServer.Models;
using Identity.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Identity.API.Areas.Oidc.Pages.Clients;

public class AddClient(IOidcClientsService oidcClientsService) : PageModel
{
    public enum ClientType
    {
        Secure,
        Web,
        ClientCredentials
    };
    
    public class ClientModel()
    {
        public bool Enabled { get; set; }
        
        [Required(ErrorMessage = "This field is mandatory")]
        public required string ClientId { get; set; }
        
        public string ClientSecret { get; set; }
        
        public ClientType Type { get; set; }
        
        [Required(ErrorMessage = "This field is mandatory")]
        public required string ClientName { get; set; }
        
        public string Description { get; set; }
        
        [Required(ErrorMessage = "This field is mandatory")]
        [Url(ErrorMessage = "Redirect uri must be a valid url")]
        public required string RedirectUri { get; set; }
        
        [Required(ErrorMessage = "This field is mandatory")]
        [Url(ErrorMessage = "Redirect uri must be a valid url")]
        public required string PostLogoutRedirectUri { get; set; }
    }
    
    [BindProperty]
    public ClientModel Input { get; set; }

    public SelectList Options { get; set; }

    public void OnGet()
    {
        var names = Enum.GetNames(typeof(ClientType)).ToList();
        Options = new SelectList(names);
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        var secureClient = IsSecureClient(Input.Type);
        if (secureClient && string.IsNullOrWhiteSpace(Input.ClientSecret))
            ModelState.AddModelError(nameof(Input.ClientSecret), "Secure clients must set client secret");
        
        if (ModelState.IsValid)
        {
            var client = new Client
            {
                Enabled = Input.Enabled,
                ClientName = Input.ClientName,
                Description = Input.Description,
                ClientId = Input.ClientId,
                AllowedGrantTypes = GetGrantTypes(Input.Type),
                RequirePkce = !secureClient,
                RequireClientSecret = secureClient,
                AllowOfflineAccess = !secureClient,
                RedirectUris = [Input.RedirectUri],
                PostLogoutRedirectUris = [Input.PostLogoutRedirectUri],
                AllowedScopes = ["openid", "profile", "email"]
            };

            if (secureClient)
                client.ClientSecrets = [new Secret(Input.ClientSecret!)];
                
            await oidcClientsService.AddClient(client, cancellationToken);

            return RedirectToPage("oidc/clients");
        }

        return Page();
    }

    private static string[] GetGrantTypes(ClientType clientType)
    {
        return clientType switch
        {
            ClientType.Secure => [GrantType.AuthorizationCode, GrantType.ClientCredentials],
            ClientType.Web => [GrantType.AuthorizationCode],
            ClientType.ClientCredentials => [GrantType.ClientCredentials],
            _ => throw new ArgumentOutOfRangeException(nameof(clientType), clientType, "Unknown client type while determining grant type")
        };
    }

    private static bool IsSecureClient(ClientType clientType)
    {
        return clientType switch
        {
            ClientType.Secure or ClientType.ClientCredentials => true,
            ClientType.Web => false,
            _ => throw new ArgumentOutOfRangeException(nameof(clientType), clientType, "Unknown client type while determining if client is secure")
        };
    }
}