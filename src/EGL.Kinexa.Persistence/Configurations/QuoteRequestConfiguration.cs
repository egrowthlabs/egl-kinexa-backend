using EGL.Kinexa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EGL.Kinexa.Persistence.Configurations;

public class QuoteRequestConfiguration : IEntityTypeConfiguration<QuoteRequest>
{
    public void Configure(EntityTypeBuilder<QuoteRequest> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CustomerName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.CustomerPhone)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.CustomerEmail)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.HasMany(x => x.QuoteItems)
            .WithOne(i => i.QuoteRequest)
            .HasForeignKey(i => i.QuoteRequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
