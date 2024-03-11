using Identity.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Data.ModelBuilders;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FirstName)
            .HasMaxLength(50)
            .IsRequired(true);
        builder.Property(x => x.LastName)
            .HasMaxLength(50)
            .IsRequired(true);
        builder.HasOne(x => x.UserProfile)
            .WithOne(x => x.User)
            .HasForeignKey<UserProfile>("UserId")
            .IsRequired(false);
    }
}