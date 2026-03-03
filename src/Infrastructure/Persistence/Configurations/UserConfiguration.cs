using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);

        // Email (Value Object → string)
        builder.Property(x => x.Email)
            .HasConversion(
                email => email.Value,
                value => Email.Create(value))
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(x => x.Email)
            .IsUnique();

        // PasswordHash (Value Object → string)
        builder.Property(x => x.PasswordHash)
            .HasConversion(
                hash => hash.Value,
                value => PasswordHash.Create(value))
            .IsRequired();

        // Enum → string (читаемо в БД)
        builder.Property(x => x.Role)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Phone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.City)
            .HasMaxLength(100);

        builder.Property(x => x.RegisteredAt)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        // Индекс для выборки пользователей по роли (например, список уборщиков)
        builder.HasIndex(x => x.Role)
            .HasDatabaseName("IX_Users_Role");
    }
}