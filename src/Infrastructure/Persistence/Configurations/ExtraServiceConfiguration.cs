using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ExtraServiceConfiguration : IEntityTypeConfiguration<ExtraService>
{
    public void Configure(EntityTypeBuilder<ExtraService> builder)
    {
        builder.ToTable("ExtraServices");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.Price)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(x => x.Unit)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.IsActive).IsRequired();
    }
}
