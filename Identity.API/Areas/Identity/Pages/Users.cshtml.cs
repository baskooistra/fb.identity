using CommunityToolkit.Diagnostics;
using Identity.Domain.Interfaces;
using Identity.Domain.Models;
using Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Identity.API.Areas.Identity.Pages
{
    public class UsersModel(UserManager<User> userManager) : PageModel
    {
        public DataTable UsersTable { get; set; } = new();

        public async Task OnGet(CancellationToken cancellationToken)
        {
            UsersTable = new DataTable();
            UsersTable.Columns.Add("UserId", typeof(string));
            UsersTable.Columns.Add("Name", typeof(string));
            UsersTable.Columns.Add("Role", typeof(string));

            var users = await userManager.Users.ToListAsync(cancellationToken);

            foreach (var user in users)
            {
                var roles = string.Join(", ", await userManager.GetRolesAsync(user));
                UsersTable.Rows.Add(user.Id, $"{user.FirstName} {user.LastName}", roles);
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            Guard.IsNotNull(user);
            await userManager.DeleteAsync(user);

            return Page();
        }
    }
}
