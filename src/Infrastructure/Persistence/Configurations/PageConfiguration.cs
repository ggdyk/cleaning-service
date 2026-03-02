using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class PageConfiguration : IEntityTypeConfiguration<Page>
{
    public void Configure(EntityTypeBuilder<Page> builder)
    {
        builder.ToTable("Pages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Slug)
            .IsRequired()
            .HasMaxLength(100);

        // Slug должен быть уникальным — одна запись на страницу
        builder.HasIndex(x => x.Slug).IsUnique();

        builder.Property(x => x.TitleRu).IsRequired().HasMaxLength(500);
        builder.Property(x => x.TitleKk).IsRequired().HasMaxLength(500);
        builder.Property(x => x.TitleEn).IsRequired().HasMaxLength(500);

        // Контент — произвольной длины (HTML или Markdown)
        builder.Property(x => x.ContentRu).IsRequired().HasColumnType("text");
        builder.Property(x => x.ContentKk).IsRequired().HasColumnType("text");
        builder.Property(x => x.ContentEn).IsRequired().HasColumnType("text");

        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();

        // Быстрая выборка активных страниц по slug
        builder.HasIndex(x => new { x.Slug, x.IsActive });
    }
}
