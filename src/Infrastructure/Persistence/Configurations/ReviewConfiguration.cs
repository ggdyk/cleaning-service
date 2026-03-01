using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId).IsRequired();

        // OrderId — опциональная связь с заказом (только ID, без навигационного свойства)
        builder.Property(x => x.OrderId);

        builder.Property(x => x.AuthorName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Rating)
            .IsRequired();

        builder.Property(x => x.ReviewText)
            .IsRequired()
            .HasMaxLength(4000);

        // Хранить enum как int для производительности
        builder.Property(x => x.ModerationStatus)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.ModeratedAt);
        builder.Property(x => x.ModeratorId);

        // Индекс для быстрой выборки одобренных отзывов
        builder.HasIndex(x => x.ModerationStatus);

        // Индекс для выборки отзывов конкретного пользователя
        builder.HasIndex(x => x.UserId);

        // Составной индекс — отзывы к заказу
        builder.HasIndex(x => x.OrderId).HasFilter("\"OrderId\" IS NOT NULL");
    }
}
