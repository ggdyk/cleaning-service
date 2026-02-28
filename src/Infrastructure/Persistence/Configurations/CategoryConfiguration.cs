using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.IconUrl).HasMaxLength(500);
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
                new { CategoryId = 1, Ru = "Уборка квартир", Kk = "Пәтерлерді тазалау", En = "Apartment cleaning" }
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
                new { CategoryId = 1, Ru = "Уборка жилых помещений", Kk = "Тұрғын үй-жайларды тазалау", En = "Residential cleaning services" }
            );
        });

        builder.HasMany(x => x.Services)
            .WithOne(s => s.Category)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(new
        {
            Id = 1,
            IconUrl = (string?)null,
            SortOrder = 1,
            IsActive = true
        });
    }
}
