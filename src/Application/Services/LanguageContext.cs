using Application.Interfaces;

namespace Application.Services;

/// <summary>
/// Scoped-реализация языкового контекста запроса.
/// LanguageMiddleware устанавливает Language в начале pipeline,
/// хендлеры читают через ILanguageContext.
/// </summary>
public class LanguageContext : ILanguageContext
{
    public string Language { get; set; } = "ru";
}
