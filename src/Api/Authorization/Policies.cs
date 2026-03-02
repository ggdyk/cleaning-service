namespace Api.Authorization;

/// <summary>
/// Именованные политики авторизации.
/// Используются в [Authorize(Policy = Policies.XYZ)] и в Program.cs при регистрации.
/// </summary>
public static class Policies
{
    /// <summary>Только Admin.</summary>
    public const string AdminOnly = "AdminOnly";

    /// <summary>Admin или Manager — управление контентом, модерация.</summary>
    public const string AdminOrManager = "AdminOrManager";

    /// <summary>Cleaner — видит назначенные заказы, меняет статус.</summary>
    public const string CleanerOnly = "CleanerOnly";

    /// <summary>Manager, Admin или Cleaner — внутренние операционные роли.</summary>
    public const string Staff = "Staff";
}
