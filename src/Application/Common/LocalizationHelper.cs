namespace Application.Common;

/// <summary>
/// Утилита для выбора нужного языкового поля из мультиязычной сущности.
/// При отсутствии перевода — fallback на русский.
/// </summary>
public static class LocalizationHelper
{
    /// <summary>
    /// Выбрать значение на нужном языке.
    /// Если перевод пустой — возвращается русская версия как fallback.
    /// </summary>
    public static string Pick(string ru, string kk, string en, string lang) =>
        lang switch
        {
            "kk" => string.IsNullOrWhiteSpace(kk) ? ru : kk,
            "en" => string.IsNullOrWhiteSpace(en) ? ru : en,
            _ => ru
        };
}
