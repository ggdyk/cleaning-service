using Application.Common.Behaviors;
using Application.Interfaces;
using Application.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Регистрация MediatR + ValidationBehavior (pipeline)
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        // Регистрация FluentValidation
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        // Регистрация IStringLocalizer<T> — читает из .resx файлов (Resources/)
        services.AddLocalization();

        // Языковой контекст запроса: middleware пишет, хендлеры читают
        services.AddScoped<LanguageContext>();
        services.AddScoped<ILanguageContext>(sp => sp.GetRequiredService<LanguageContext>());

        return services;
    }
}