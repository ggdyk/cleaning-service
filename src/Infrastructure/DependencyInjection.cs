using Application.Features.Calculator.CalculatePrice;
using Application.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null);
                });
        });

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<IFAQRepository, FAQRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<ICallbackRequestRepository, CallbackRequestRepository>();
        services.AddScoped<ICalculatorSettingsRepository, CalculatorSettingsRepository>();
        services.AddScoped<IExtraServiceRepository, ExtraServiceRepository>();
        services.AddScoped<IPageRepository, PageRepository>();
        services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();

        // Services
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtService, JwtService>();

        // Настройки калькулятора по умолчанию (fallback если нет записи в БД)
        services.Configure<CalculatorDefaultSettings>(
            configuration.GetSection(CalculatorDefaultSettings.SectionName));

        return services;
    }
}