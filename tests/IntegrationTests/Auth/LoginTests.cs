using System.Net;
using System.Net.Http.Json;
using IntegrationTests.Helpers;
using IntegrationTests.Setup;

namespace IntegrationTests.Auth;

[Collection("Integration")]
public class LoginTests
{
    private readonly HttpClient _client;

    public LoginTests(IntegrationWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_ValidCredentials_Returns200WithAccessToken()
    {
        // Arrange: сначала регистрируем пользователя
        var email = $"{Guid.NewGuid()}@test.com";
        const string password = "Test@1234";
        await _client.PostAsJsonAsync("/api/auth/register", TestHelpers.BuildRegisterRequest(email, password));

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new { email, password });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        var token = body.GetProperty("accessToken").GetString();
        Assert.NotNull(token);
        Assert.NotEmpty(token);

        // RefreshToken тоже должен быть
        var refresh = body.GetProperty("refreshToken").GetString();
        Assert.NotNull(refresh);
        Assert.NotEmpty(refresh);
    }

    [Fact]
    public async Task Login_WrongPassword_Returns401()
    {
        // Arrange: регистрируем пользователя
        var email = $"{Guid.NewGuid()}@test.com";
        await _client.PostAsJsonAsync("/api/auth/register", TestHelpers.BuildRegisterRequest(email));

        // Act: логинимся с неверным паролем
        var response = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new { email, password = "WrongPassword!" });

        // Assert: LoginHandler бросает UnauthorizedAccessException → 401
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_UnknownEmail_Returns401()
    {
        // Act: логинимся с email, которого нет в БД
        var response = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new { email = "nobody@nowhere.com", password = "AnyPassword!" });

        // Assert: LoginHandler бросает UnauthorizedAccessException → 401
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
