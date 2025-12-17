using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<User> Users => Set<User>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<ExtraService> ExtraServices => Set<ExtraService>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderService> OrderServices => Set<OrderService>();
    public DbSet<OrderExtraService> OrderExtraServices => Set<OrderExtraService>();
    public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<FAQ> FAQs => Set<FAQ>();
    public DbSet<CallBackRequest> CallBackRequests => Set<CallBackRequest>();
    public DbSet<Page> Pages => Set<Page>();
    public DbSet<CalculatorSettings> CalculatorSettings => Set<CalculatorSettings>();
    public DbSet<OrderStatusHistory> OrderStatusHistories => Set<OrderStatusHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Подключаем все IEntityTypeConfiguration из сборки Infrastructure
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}