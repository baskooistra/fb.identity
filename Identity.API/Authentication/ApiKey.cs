using AspNetCore.Authentication.ApiKey;
using System.Security.Claims;

namespace Identity.API.Authentication
{
    public class ApiKey : IApiKey
    {
        public required string Key { get; set; }
        public required string OwnerName { get; set; }
        public IReadOnlyCollection<Claim> Claims => [];
    }
}
