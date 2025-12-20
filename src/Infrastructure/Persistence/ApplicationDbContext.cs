using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSet для User
    public DbSet<User> Users { get; set; }
    
    // DbSet для RefreshToken
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Конфигурация для User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Email)
                .HasConversion(
                    email => email.Value,
                    value => Domain.ValueObjects.Email.Create(value))
                .HasMaxLength(255)
                .IsRequired();
            
            entity.HasIndex(e => e.Email).IsUnique();
            
            entity.Property(e => e.PasswordHash)
                .HasConversion(
                    hash => hash.Value,
                    value => Domain.ValueObjects.PasswordHash.Create(value))
                .HasMaxLength(60)
                .IsRequired();
            
            entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Phone).HasMaxLength(20).IsRequired();
            entity.Property(e => e.City).HasMaxLength(100);
            
            entity.Property(e => e.Role)
                .HasConversion<int>()
                .IsRequired();
            
            entity.Property(e => e.RegisteredAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            
            entity.ToTable("Users");
        });

        // Конфигурация для RefreshToken
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.Token).HasMaxLength(500).IsRequired();
            entity.Property(e => e.ExpiresAt).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsRevoked).IsRequired();
            entity.Property(e => e.RevokedAt);
            entity.Property(e => e.ReplacedByToken).HasMaxLength(500);

            entity.HasIndex(e => e.Token).IsUnique();
            entity.HasIndex(e => e.UserId);

            entity.ToTable("RefreshTokens");
        });
    }
}