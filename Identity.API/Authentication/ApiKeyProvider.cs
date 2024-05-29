using AspNetCore.Authentication.ApiKey;
using Microsoft.Extensions.Options;
using Serilog;

namespace Identity.API.Authentication
{
    public class ApiKeyProvider(IOptionsSnapshot<AuthenticationConfiguration> options) : IApiKeyProvider
    {
        public Task<IApiKey> ProvideAsync(string key)
        {
            try
            {
                Log.Information("Validating API key {key}", key);
                IApiKey apiKey = options.Value.ApiKeys.Single(x => x.Key == key);
                return Task.FromResult(apiKey);
            }
            catch (Exception e)
            {
                Log.Error(e, "Unable to validate api key {apiKey}. Please check api key configuration", key);
                throw;
            }
            throw new NotImplementedException();
        }
    }
}
