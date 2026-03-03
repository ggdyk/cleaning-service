using System.Globalization;
using Application.Services;

namespace Api.Middleware;

/// <summary>
/// Middleware определяет язык запроса и записывает его в LanguageContext.
///
/// Приоритет:
///   1. Query-параметр ?lang=ru  (явный выбор, приоритет)
///   2. Заголовок Accept-Language  (стандартный браузерный механизм)
///   3. Русский по умолчанию
///
/// Поддерживаемые коды: "ru", "kk", "en".
/// </summary>
public sealed class LanguageMiddleware
{
    private static readonly HashSet<string> Supported = ["ru", "kk", "en"];
    private const string Default = "ru";

    private readonly RequestDelegate _next;

    public LanguageMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, LanguageContext languageContext)
    {
        var language = Detect(context.Request);
        languageContext.Language = language;

        // Устанавливаем культуру потока — IStringLocalizer<T> читает CultureInfo.CurrentUICulture
        var culture = new CultureInfo(language);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        await _next(context);
    }

    private static string Detect(HttpRequest request)
    {
        // 1. ?lang= query param
        if (request.Query.TryGetValue("lang", out var qv))
        {
            var q = qv.ToString().ToLowerInvariant().Trim();
            if (Supported.Contains(q))
                return q;
        }

        // 2. Accept-Language header (берём первый тег, отбрасываем регион: "kk-KZ" → "kk")
        var header = request.Headers.AcceptLanguage.ToString();
        if (!string.IsNullOrEmpty(header))
        {
            var primary = header.Split(',')[0].Split('-')[0].ToLowerInvariant().Trim();
            if (Supported.Contains(primary))
                return primary;
        }

        return Default;
    }
}
