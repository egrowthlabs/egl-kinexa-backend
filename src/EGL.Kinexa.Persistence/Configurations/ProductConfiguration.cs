using EGL.Kinexa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EGL.Kinexa.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Slug)
            .IsRequired()
            .HasMaxLength(250);

        builder.HasIndex(x => x.Slug)
            .IsUnique();

        builder.Property(x => x.Description)
            .IsRequired();

        builder.Property(x => x.SeoKeywords)
            .HasMaxLength(500);

        builder.Property(x => x.UsageIndications)
            .HasMaxLength(1000);

        builder.Property(x => x.Material)
            .HasMaxLength(200);

        builder.Property(x => x.MaterialType)
            .HasMaxLength(200);

        builder.Property(x => x.Measurements)
            .HasMaxLength(500);

        builder.Property(x => x.SpecificInstruments)
            .HasMaxLength(1000);

        builder.Property(x => x.Competitors)
            .HasMaxLength(1000);

        builder.Property(x => x.ImageUrl)
            .HasMaxLength(500);

        builder.HasOne(x => x.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.MedicalBranch)
            .WithMany(m => m.Products)
            .HasForeignKey(x => x.MedicalBranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.CategoryId);
        builder.HasIndex(x => x.MedicalBranchId);
    }
}
