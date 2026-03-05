using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Setup;

/// <summary>
/// Общая фабрика для интеграционных тестов.
/// Заменяет PostgreSQL на SQLite in-memory, чтобы тесты не требовали запущенного сервера БД.
/// Одна фабрика — одна БД — все тесты коллекции "Integration".
/// </summary>
public class IntegrationWebApplicationFactory
    : WebApplicationFactory<Program>, IAsyncLifetime
{
    // Shared connection: держим её открытой на всё время коллекции тестов.
    // In-memory SQLite живёт ровно столько, сколько открыт хотя бы один connection.
    private readonly SqliteConnection _connection = new("Filename=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Помечаем окружение "Testing" — Program.cs проверяет его, чтобы не делать HTTPS-redirect
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // EF Core 8+ регистрирует опции через generic IDbContextOptionsConfiguration<TContext>
            // (internal тип). При двух вызовах AddDbContext оба конфига (Npgsql + SQLite)
            // применяются одновременно → ошибка "multiple providers".
            //
            // Решение: удаляем ВСЕ дескрипторы, чей ServiceType содержит ApplicationDbContext
            // как generic аргумент (IDbContextOptionsConfiguration<ApplicationDbContext>,
            // DbContextOptions<ApplicationDbContext>), а затем заново регистрируем с SQLite.

            var descriptorsToRemove = services
                .Where(d =>
                    d.ServiceType == typeof(ApplicationDbContext) ||
                    d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>) ||
                    (d.ServiceType.IsGenericType &&
                     d.ServiceType.GenericTypeArguments.Contains(typeof(ApplicationDbContext))))
                .ToList();

            foreach (var d in descriptorsToRemove)
                services.Remove(d);

            // Единственная конфигурация — SQLite in-memory
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(_connection));
        });
    }

    /// <summary>
    /// Вызывается xUnit перед первым тестом в коллекции.
    /// Открывает соединение и создаёт схему БД (включая seed-данные из HasData).
    /// </summary>
    public async Task InitializeAsync()
    {
        _connection.Open();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    /// <summary>
    /// Вызывается xUnit после последнего теста в коллекции.
    /// </summary>
    async Task IAsyncLifetime.DisposeAsync()
    {
        await _connection.DisposeAsync();
    }
}
