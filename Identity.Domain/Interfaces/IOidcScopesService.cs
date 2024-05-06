using Duende.IdentityServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Interfaces
{
    public interface IOidcScopesService
    {
        Task<IEnumerable<ApiScope>> GetScopes(CancellationToken cancellationToken);
        Task<ApiScope> AddScope(ApiScope scope, CancellationToken cancellationToken);
        Task DeleteScope(ApiScope scope, CancellationToken cancellationToken);
    }
}
