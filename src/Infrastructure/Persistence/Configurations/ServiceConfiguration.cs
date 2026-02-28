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

        builder.OwnsOne(x => x.Name, name =>
        {
            name.Property(n => n.Ru)
                .HasColumnName("name_ru")
                .HasMaxLength(255)
                .IsRequired();

            name.Property(n => n.Kk)
                .HasColumnName("name_kk")
                .HasMaxLength(255)
                .IsRequired();

            name.Property(n => n.En)
                .HasColumnName("name_en")
                .HasMaxLength(255)
                .IsRequired();

            name.HasData(
                new { ServiceId = 1, Ru = "Генеральная уборка",    Kk = "Жалпы жинау",           En = "General cleaning"     },
                new { ServiceId = 2, Ru = "Поддерживающая уборка", Kk = "Қолдаушы жинау",         En = "Maintenance cleaning" }
            );
        });

        builder.OwnsOne(x => x.Description, desc =>
        {
            desc.Property(d => d.Ru)
                .HasColumnName("description_ru")
                .IsRequired();

            desc.Property(d => d.Kk)
                .HasColumnName("description_kk")
                .IsRequired();

            desc.Property(d => d.En)
                .HasColumnName("description_en")
                .IsRequired();

            desc.HasData(
                new { ServiceId = 1, Ru = "Полная уборка помещения", Kk = "Үй-жайды толық жинау",  En = "Full cleaning of the premises" },
                new { ServiceId = 2, Ru = "Регулярная уборка",       Kk = "Тұрақты жинау қызметі", En = "Regular cleaning service"     }
            );
        });

        builder.HasData(
            new
            {
                Id = 1,
                CategoryId = 1,
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
                CategoryId = 1,
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
