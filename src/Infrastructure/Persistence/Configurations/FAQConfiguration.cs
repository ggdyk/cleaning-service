using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class FAQConfiguration : IEntityTypeConfiguration<FAQ>
{
    public void Configure(EntityTypeBuilder<FAQ> builder)
    {
        builder.ToTable("FAQs");

        builder.HasKey(x => x.Id);

        // Вопросы (три языка)
        builder.Property(x => x.QuestionRu).IsRequired().HasMaxLength(1000);
        builder.Property(x => x.QuestionKk).IsRequired().HasMaxLength(1000);
        builder.Property(x => x.QuestionEn).IsRequired().HasMaxLength(1000);

        // Ответы (три языка)
        builder.Property(x => x.AnswerRu).IsRequired().HasMaxLength(5000);
        builder.Property(x => x.AnswerKk).IsRequired().HasMaxLength(5000);
        builder.Property(x => x.AnswerEn).IsRequired().HasMaxLength(5000);

        builder.Property(x => x.SortOrder).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt);

        // Индекс для быстрой выборки активных записей в нужном порядке
        builder.HasIndex(x => new { x.IsActive, x.SortOrder });
    }
}
