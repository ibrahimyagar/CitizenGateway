using CitizenGateway.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CitizenGateway.Infrastructure.Persistence.Configurations;

public sealed class AuditLogEntryConfiguration : IEntityTypeConfiguration<AuditLogEntry>
{
    public void Configure(EntityTypeBuilder<AuditLogEntry> builder)
    {
        builder.ToTable("audit_logs");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserRole).HasConversion<string>().HasMaxLength(32);
        builder.Property(x => x.AccessedCitizenTcNo).HasMaxLength(11).IsRequired();
        builder.Property(x => x.AccessedEndpoint).HasMaxLength(256).IsRequired();
        builder.Property(x => x.IpAddress).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Timestamp).IsRequired();

        // Audit sorguları genelde zamana göre — indeks okuma performansına yardımcı olur.
        builder.HasIndex(x => x.Timestamp);
    }
}
