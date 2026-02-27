using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class OrderStatusHistoryConfiguration : IEntityTypeConfiguration<OrderStatusHistory>
{
    public void Configure(EntityTypeBuilder<OrderStatusHistory> builder)
    {
        builder.ToTable("OrderStatusHistory");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderId).IsRequired();

        // Enum → string для читаемости в БД
        builder.Property(x => x.PreviousStatus)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.NewStatus)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.ChangedAt).IsRequired();
        builder.Property(x => x.ChangedByUserId).IsRequired();
        builder.Property(x => x.Comment).HasMaxLength(1000);
    }
}
