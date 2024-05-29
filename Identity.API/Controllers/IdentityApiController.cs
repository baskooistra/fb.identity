using Identity.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;
using CommunityToolkit.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using AspNetCore.Authentication.ApiKey;

namespace Identity.API.Controllers
{
    [Route("api/identity")]
    [ApiController]
    [Authorize(AuthenticationSchemes = ApiKeyDefaults.AuthenticationScheme)]
    public class IdentityApiController(UserManager<User> userManager) : ControllerBase
    {
        [HttpPost("account/{id}/confirmation")]
        public async Task<string> Post([FromRoute]string id, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(id);
            
            Guard.IsNotNull(user, nameof(user));
            Guard.IsFalse(user.EmailConfirmed, nameof(user.EmailConfirmed), "User email is already confirmed");

            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
            "/Account/ConfirmEmail",
            pageHandler: null,
                values: new { area = "Identity", userId = id, code, returnUrl = Url.Content("~/") },
                protocol: Request.Scheme);

            Guard.IsNotNullOrWhiteSpace(callbackUrl);

            return callbackUrl;
        }
    }
}
