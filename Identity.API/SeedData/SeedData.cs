using CommunityToolkit.Diagnostics;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Models;
using Identity.API.Constants;
using Identity.Domain.Models;
using Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Identity.API.SeedData
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, bool migrateDatabase = false)
        {
            var context = serviceProvider.GetService<IdentityContext>();
            var configurationContext = serviceProvider.GetService<ConfigurationDbContext>();
            var persistedGrantContext = serviceProvider.GetService<PersistedGrantDbContext>();
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            Guard.IsNotNull(context);
            Guard.IsNotNull(configurationContext);
            Guard.IsNotNull(persistedGrantContext);
            Guard.IsNotNull(roleManager);
            Guard.IsNotNull(userManager);

            if (migrateDatabase) 
            {
                await context.Database.MigrateAsync();
                await configurationContext.Database.MigrateAsync();
                await persistedGrantContext.Database.MigrateAsync();
            }

            string[] roles = [Roles.Owner, Roles.User];

            foreach (string roleName in roles)
            {
                var role = await roleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                    if (!result.Succeeded)
                    {
                        HandleIdentityError();
                        return;
                    }
                }
            }

            var userData = new User
            {
                FirstName = "Bas",
                LastName = "Kooistra",
                Email = "bas.kooistra1986@gmail.com",
                NormalizedEmail = "BAS.KOOISTRA1986@GMAIL.COM",
                UserName = "bas.kooistra",
                NormalizedUserName = "BAS.KOOISTRA",
                PhoneNumber = "+31615374420",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            var user = await userManager.FindByNameAsync(userData.UserName);
            if (user == null)
            {
                var result = await userManager.CreateAsync(userData, "P@ssw0rd_2024");
                if (!result.Succeeded)
                {
                    HandleIdentityError();
                    return;
                }
                    
                user = await userManager.FindByNameAsync(userData.UserName);
                Guard.IsNotNull(user);
                
                result = await userManager.AddToRolesAsync(user, roles);
                if (!result.Succeeded)
                    HandleIdentityError();
            }

            if (!configurationContext.IdentityResources.Any())
            {
                Log.Debug("IdentityResources being populated");
                foreach (var resource in IdentityResources)
                {
                    configurationContext.IdentityResources.Add(resource.ToEntity());
                }
                await configurationContext.SaveChangesAsync();
            }
        }

        private static IEnumerable<IdentityResource> IdentityResources =>
        [
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email()
        ];

        private static void HandleIdentityError()
        {
            Log.Warning("An error occured while seeding default identity data, see the errors for details");
        }
    }
}
