using Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(e => e.BirthDate).IsRequired()
            .HasColumnType("timestamp without time zone");
        builder.Property(e => e.LastName).IsRequired();
        builder.Property(e => e.Name).IsRequired();
        builder.Property(e => e.RefreshToken);
    }
}