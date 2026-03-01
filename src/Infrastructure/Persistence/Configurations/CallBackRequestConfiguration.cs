using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class CallBackRequestConfiguration : IEntityTypeConfiguration<CallBackRequest>
{
    public void Configure(EntityTypeBuilder<CallBackRequest> builder)
    {
        builder.ToTable("CallbackRequests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Phone)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.PreferredTime)
            .HasMaxLength(200);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.CreatedAt).IsRequired();

        // Индекс: выборка новых заявок по дате
        builder.HasIndex(x => new { x.Status, x.CreatedAt });
    }
}
