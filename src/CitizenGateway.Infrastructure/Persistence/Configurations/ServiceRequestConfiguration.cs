using CitizenGateway.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CitizenGateway.Infrastructure.Persistence.Configurations;

public sealed class ServiceRequestConfiguration : IEntityTypeConfiguration<ServiceRequest>
{
    public void Configure(EntityTypeBuilder<ServiceRequest> builder)
    {
        builder.ToTable("service_requests");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.RequestType).HasConversion<string>().HasMaxLength(32);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(32);
        builder.Property(x => x.TargetService).HasConversion<string>().HasMaxLength(32);
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasOne(x => x.Citizen)
            .WithMany()
            .HasForeignKey(x => x.CitizenId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.CitizenId);
    }
}
