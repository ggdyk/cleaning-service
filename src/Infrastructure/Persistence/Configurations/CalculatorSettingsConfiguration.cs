using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class CalculatorSettingsConfiguration : IEntityTypeConfiguration<CalculatorSettings>
{
    public void Configure(EntityTypeBuilder<CalculatorSettings> builder)
    {
        builder.ToTable("CalculatorSettings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CityId).IsRequired();

        builder.Property(x => x.PricePerSquareMeter)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(x => x.PricePerBathroom)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(x => x.MinimumOrderAmount)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(x => x.UpdatedAt).IsRequired();

        // Уникальный индекс — одни настройки на город
        builder.HasIndex(x => x.CityId).IsUnique();

        // Seed: дефолтные настройки для города с Id=1
        builder.HasData(new
        {
            Id = 1,
            CityId = 1,
            PricePerSquareMeter = 50m,
            PricePerBathroom = 1000m,
            MinimumOrderAmount = 3000m,
            UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}
