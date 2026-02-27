using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderNumber)
            .IsRequired()
            .HasMaxLength(30);

        builder.HasIndex(x => x.OrderNumber)
            .IsUnique();

        builder.Property(x => x.ClientId).IsRequired();
        builder.Property(x => x.CleanerId); // nullable — назначается позже
        builder.Property(x => x.CityId).IsRequired();
        builder.Property(x => x.TimeSlotId).IsRequired();

        // Адрес
        builder.Property(x => x.Street).IsRequired().HasMaxLength(255);
        builder.Property(x => x.House).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Apartment).HasMaxLength(20);
        builder.Property(x => x.Entrance).HasMaxLength(20);
        builder.Property(x => x.Floor).HasMaxLength(20);
        builder.Property(x => x.DoorCode).HasMaxLength(50);

        // Параметры помещения
        builder.Property(x => x.Area).IsRequired();
        builder.Property(x => x.Bathrooms).IsRequired();
        builder.Property(x => x.Comment).HasMaxLength(1000);

        // Цена и статус
        builder.Property(x => x.TotalPrice)
            .HasPrecision(10, 2)
            .IsRequired();

        // Enum → string (читаемо в БД, как в UserConfiguration)
        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();

        // Связи с дочерними коллекциями
        builder.HasMany(x => x.Services)
            .WithOne()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.ExtraServices)
            .WithOne()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.StatusHistory)
            .WithOne()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}