using CommunityToolkit.Diagnostics;
using Duende.IdentityServer.Models;
using Identity.Domain.Models;
using Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Identity.API.SeedData
{
    public class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<IdentityContext>();
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            Guard.IsNotNull(context);
            Guard.IsNotNull(roleManager);
            Guard.IsNotNull(userManager);

            await context.Database.MigrateAsync();

            string[] roles = ["Owner", "User"];

            foreach (string roleName in roles)
            {
                var role = await roleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                    if (!result.Succeeded)
                    {
                        HandleIdentityError(result);
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
                    HandleIdentityError(result);
                    return;
                }
                    
                user = await userManager.FindByNameAsync(userData.UserName);
                Guard.IsNotNull(user);
                
                result = await userManager.AddToRolesAsync(user, roles);
                if (!result.Succeeded)
                    HandleIdentityError(result);
            }
        }

        private static void HandleIdentityError(IdentityResult result)
        {
            Log.Warning("An error occured while seeding default identity data, see the errors for details", result.Errors);
        }
    }
}
