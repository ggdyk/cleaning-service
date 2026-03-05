using System.Net.Http.Json;

namespace IntegrationTests.Helpers;

/// <summary>
/// Вспомогательные методы для интеграционных тестов.
/// </summary>
public static class TestHelpers
{
    /// <summary>Формирует корректный запрос на регистрацию.</summary>
    public static object BuildRegisterRequest(string email, string password = "Test@1234") => new
    {
        email,
        password,
        firstName = "Тест",
        lastName = "Тестов",
        phone = "+77001234567"
    };

    /// <summary>
    /// Регистрирует нового пользователя и возвращает его AccessToken.
    /// Использует уникальный email, чтобы тесты не конфликтовали между собой.
    /// </summary>
    public static async Task<string> RegisterAndLoginAsync(
        HttpClient client,
        string email,
        string password = "Test@1234")
    {
        var registerResponse = await client.PostAsJsonAsync(
            "/api/auth/register",
            BuildRegisterRequest(email, password));

        registerResponse.EnsureSuccessStatusCode();

        var loginResponse = await client.PostAsJsonAsync(
            "/api/auth/login",
            new { email, password });

        loginResponse.EnsureSuccessStatusCode();

        var body = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("accessToken").GetString()!;
    }
}
