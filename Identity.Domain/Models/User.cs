using Microsoft.AspNetCore.Identity;

namespace Identity.Domain.Models;

public class User : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public virtual UserProfile? UserProfile { get; set; }
}