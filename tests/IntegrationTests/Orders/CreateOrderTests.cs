using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using IntegrationTests.Helpers;
using IntegrationTests.Setup;

namespace IntegrationTests.Orders;

[Collection("Integration")]
public class CreateOrderTests
{
    private readonly HttpClient _client;

    public CreateOrderTests(IntegrationWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateOrder_WithValidToken_Returns201()
    {
        // Arrange: регистрируемся + логинимся → получаем токен
        var email = $"{Guid.NewGuid()}@test.com";
        var token = await TestHelpers.RegisterAndLoginAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Используем ServiceId=1, CityId=1 — оба есть в seed-данных (HasData в конфигурациях EF Core)
        var request = new
        {
            cityId = 1,
            timeSlotId = 1,
            street = "ул. Тестовая",
            house = "10",
            area = 50.0,
            bathrooms = 1,
            services = new[] { new { serviceId = 1, quantity = 1.0 } },
            extraServices = Array.Empty<object>()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/orders", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(body.GetProperty("id").GetInt32() > 0);
        Assert.False(string.IsNullOrEmpty(body.GetProperty("orderNumber").GetString()));
        Assert.True(body.GetProperty("totalPrice").GetDecimal() > 0);
    }

    [Fact]
    public async Task CreateOrder_WithoutToken_Returns401()
    {
        // Arrange: убеждаемся, что заголовок авторизации не установлен
        _client.DefaultRequestHeaders.Authorization = null;

        var request = new
        {
            cityId = 1,
            timeSlotId = 1,
            street = "ул. Тестовая",
            house = "10",
            area = 50.0,
            bathrooms = 1,
            services = new[] { new { serviceId = 1, quantity = 1.0 } },
            extraServices = Array.Empty<object>()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/orders", request);

        // Assert: [Authorize] вернёт 401 без токена
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateOrder_WithNonExistentService_Returns404()
    {
        // Arrange
        var email = $"{Guid.NewGuid()}@test.com";
        var token = await TestHelpers.RegisterAndLoginAsync(_client, email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new
        {
            cityId = 1,
            timeSlotId = 1,
            street = "ул. Тестовая",
            house = "10",
            area = 50.0,
            bathrooms = 1,
            services = new[] { new { serviceId = 99999, quantity = 1.0 } }, // несуществующая услуга
            extraServices = Array.Empty<object>()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/orders", request);

        // Assert: CreateOrderHandler бросает NotFoundException → 404
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
