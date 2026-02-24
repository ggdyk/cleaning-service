using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class OrderExtraServiceConfiguration : IEntityTypeConfiguration<OrderExtraService>
{
    public void Configure(EntityTypeBuilder<OrderExtraService> builder)
    {
        builder.ToTable("OrderExtraServices");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderId).IsRequired();
        builder.Property(x => x.ExtraServiceId).IsRequired();

        // Снимок названия и цены на момент создания заказа
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.UnitPrice)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(x => x.Quantity).IsRequired();

        // TotalPrice — вычисляемое, в БД не хранится
        builder.Ignore(x => x.TotalPrice);

        builder.Property(x => x.CreatedAt).IsRequired();
    }
}
