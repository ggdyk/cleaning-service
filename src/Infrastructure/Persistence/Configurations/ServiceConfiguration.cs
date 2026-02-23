using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("Services");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CategoryId).IsRequired();

        builder.Property(x => x.BasePrice)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(x => x.Unit)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.MinArea);
        builder.Property(x => x.DurationMinutes);

        builder.Property(x => x.SortOrder).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();

        // Name (LocalizedString) — HasData для owned type вызывается внутри OwnsOne
        builder.OwnsOne(x => x.Name, name =>
        {
            name.Property(n => n.Ru)
                .HasColumnName("name_ru")
                .HasMaxLength(255)
                .IsRequired();

            name.Property(n => n.En)
                .HasColumnName("name_en")
                .HasMaxLength(255)
                .IsRequired();

            name.HasData(
                new { ServiceId = 1, Ru = "Генеральная уборка",     En = "General cleaning"     },
                new { ServiceId = 2, Ru = "Поддерживающая уборка",  En = "Maintenance cleaning" }
            );
        });

        // Description (LocalizedString)
        builder.OwnsOne(x => x.Description, desc =>
        {
            desc.Property(d => d.Ru)
                .HasColumnName("description_ru")
                .IsRequired();

            desc.Property(d => d.En)
                .HasColumnName("description_en")
                .IsRequired();

            desc.HasData(
                new { ServiceId = 1, Ru = "Полная уборка помещения", En = "Full cleaning of the premises" },
                new { ServiceId = 2, Ru = "Регулярная уборка",       En = "Regular cleaning service"     }
            );
        });

        // Seed data — только скалярные свойства родительской сущности
        builder.HasData(
            new
            {
                Id = 1,
                CategoryId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                BasePrice = 1000m,
                Unit = "service",
                MinArea = (double?)null,
                DurationMinutes = (int?)120,
                SortOrder = 1,
                IsActive = true
            },
            new
            {
                Id = 2,
                CategoryId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                BasePrice = 500m,
                Unit = "sqm",
                MinArea = (double?)30.0,
                DurationMinutes = (int?)60,
                SortOrder = 2,
                IsActive = true
            }
        );
    }
}