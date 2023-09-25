using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configuration;

public class RoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.HasData(
        new ApplicationRole
        {
            Id = 1,
            Name = "User",
            NormalizedName = "USER"
        },
        new ApplicationRole
        {
            Id = 2,
            Name = "Manager",
            NormalizedName = "MANAGER"
        },
        new ApplicationRole
        {
            Id = 3,
            Name = "Admin",
            NormalizedName = "ADMIN"
        }
        );
    }
}
