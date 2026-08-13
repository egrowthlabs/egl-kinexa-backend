using EGL.Kinexa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EGL.Kinexa.Persistence.Configurations;

public class MedicalBranchConfiguration : IEntityTypeConfiguration<MedicalBranch>
{
    public void Configure(EntityTypeBuilder<MedicalBranch> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Slug)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(x => x.Slug)
            .IsUnique();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.IconUrl)
            .HasMaxLength(500);
    }
}
