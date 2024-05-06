using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Models;
using Identity.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Repositories
{
    internal class OidcScopesRepository(
        ConfigurationDbContext context) : IOidcScopesService
    {
        async Task<ApiScope> IOidcScopesService.AddScope(ApiScope scope, CancellationToken cancellationToken)
        {
            var scopeEntity = context.ApiScopes.Add(scope.ToEntity());
            await context.SaveChangesAsync(cancellationToken);
            return scopeEntity.Entity.ToModel();
        }

        async Task IOidcScopesService.DeleteScope(ApiScope scope, CancellationToken cancellationToken)
        {
            var scopeEntity = await context.ApiScopes
                .Where(x => x.Name == scope.Name)
                .SingleOrDefaultAsync(cancellationToken);

            if (scopeEntity == null)
                throw new KeyNotFoundException("No client found matching client id");

            context.Entry(scopeEntity).State = EntityState.Deleted;
            await context.SaveChangesAsync(cancellationToken);
        }

        async Task<IEnumerable<ApiScope>> IOidcScopesService.GetScopes(CancellationToken cancellationToken)
        {
            return await context.ApiScopes
                .Select(scope => scope.ToModel())
                .ToListAsync(cancellationToken);
        }
    }
}
