using Application.Interfaces;
using Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Регистрация MediatR
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        // Регистрация FluentValidation
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        // Языковой контекст запроса: middleware пишет, хендлеры читают
        services.AddScoped<LanguageContext>();
        services.AddScoped<ILanguageContext>(sp => sp.GetRequiredService<LanguageContext>());

        return services;
    }
}