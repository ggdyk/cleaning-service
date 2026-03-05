using System.Net;
using System.Net.Http.Json;
using IntegrationTests.Helpers;
using IntegrationTests.Setup;

namespace IntegrationTests.Auth;

[Collection("Integration")]
public class RegisterTests
{
    private readonly HttpClient _client;

    public RegisterTests(IntegrationWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ValidRequest_Returns200AndUserId()
    {
        var email = $"{Guid.NewGuid()}@test.com";
        var request = TestHelpers.BuildRegisterRequest(email);

        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(body.GetProperty("userId").GetInt32() > 0);
        Assert.Equal(email, body.GetProperty("email").GetString());
    }

    [Fact]
    public async Task Register_ReturnsFirstAndLastName()
    {
        var email = $"{Guid.NewGuid()}@test.com";
        var request = new
        {
            email,
            password = "Test@1234",
            firstName = "Иван",
            lastName = "Иванов",
            phone = "+77001234567"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Иван", body.GetProperty("firstName").GetString());
        Assert.Equal("Иванов", body.GetProperty("lastName").GetString());
    }

    [Fact]
    public async Task Register_CreatedUser_CanLogin()
    {
        // Регистрируем пользователя
        var email = $"{Guid.NewGuid()}@test.com";
        const string password = "Test@1234";
        var registerResponse = await _client.PostAsJsonAsync(
            "/api/auth/register",
            TestHelpers.BuildRegisterRequest(email, password));
        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

        // Убеждаемся, что зарегистрированный пользователь успешно входит
        var loginResponse = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new { email, password });
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginBody = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.NotEmpty(loginBody.GetProperty("accessToken").GetString()!);
    }
}
