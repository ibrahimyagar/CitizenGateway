using CitizenGateway.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CitizenGateway.Infrastructure.Persistence.Configurations;

public sealed class CitizenConfiguration : IEntityTypeConfiguration<Citizen>
{
    public void Configure(EntityTypeBuilder<Citizen> builder)
    {
        builder.ToTable("citizens");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.TcNo).HasMaxLength(11).IsRequired();
        builder.HasIndex(x => x.TcNo).IsUnique();

        builder.Property(x => x.AdSoyad).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Telefon).HasMaxLength(32).IsRequired();
        builder.Property(x => x.DogumTarihi).IsRequired();
    }
}
