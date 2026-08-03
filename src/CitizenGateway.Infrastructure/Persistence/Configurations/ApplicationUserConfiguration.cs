using CitizenGateway.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CitizenGateway.Infrastructure.Persistence.Configurations;

public sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("users");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Username).HasMaxLength(128).IsRequired();
        builder.HasIndex(x => x.Username).IsUnique();

        builder.Property(x => x.DisplayName).HasMaxLength(128).IsRequired();

        builder.Property(x => x.PasswordHash).HasMaxLength(512).IsRequired();
        builder.Property(x => x.Role).HasConversion<string>().HasMaxLength(32);

        builder.HasOne(x => x.LinkedCitizen)
            .WithMany()
            .HasForeignKey(x => x.LinkedCitizenId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
