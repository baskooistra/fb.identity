namespace Identity.API.Authentication
{
    public class AuthenticationConfiguration
    {
        public required IEnumerable<ApiKey> ApiKeys { get; set; }
    }
}
