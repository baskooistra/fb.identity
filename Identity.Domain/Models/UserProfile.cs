namespace Identity.Domain.Models;

public class UserProfile
{
    public required string UserId { get; set; }
    public required DateOnly BirthDate { get; set; }
    public required Address Address { get; set; }
    public virtual required User User { get; set; }
}