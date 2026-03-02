namespace Application.Interfaces;

/// <summary>
/// Предоставляет доступ к текущему языку запроса.
/// Устанавливается middleware на каждый HTTP-запрос.
/// </summary>
public interface ILanguageContext
{
    /// <summary>Код языка: "ru", "kk" или "en". По умолчанию "ru".</summary>
    string Language { get; }
}
