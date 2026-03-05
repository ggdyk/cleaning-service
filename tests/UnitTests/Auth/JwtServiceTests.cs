using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace UnitTests.Auth;

public class JwtServiceTests
{
    // Тестовые настройки JWT — ключ минимум 32 символа для HMAC-SHA256
    private const string TestKey      = "test-secret-key-minimum-32-chars-for-hmac";
    private const string TestIssuer   = "TestIssuer";
    private const string TestAudience = "TestAudience";

    private readonly JwtService _sut;

    public JwtServiceTests()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"]      = TestKey,
                ["Jwt:Issuer"]   = TestIssuer,
                ["Jwt:Audience"] = TestAudience
            })
            .Build();

        _sut = new JwtService(config);
    }

    // ─────────────────────── GenerateAccessToken ────────────────────────────

    [Fact]
    public void GenerateAccessToken_ValidUser_ReturnsNonEmptyString()
    {
        var user = CreateUser(id: 7);

        var token = _sut.GenerateAccessToken(user);

        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public void GenerateAccessToken_TokenContainsUserId()
    {
        var user = CreateUser(id: 42);

        var token = _sut.GenerateAccessToken(user);
        var parsed = ParseToken(token);

        var sub = parsed.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
        Assert.Equal("42", sub);
    }

    [Fact]
    public void GenerateAccessToken_TokenContainsEmail()
    {
        var user = CreateUser(email: "admin@test.com");

        var token = _sut.GenerateAccessToken(user);
        var parsed = ParseToken(token);

        var email = parsed.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value;
        Assert.Equal("admin@test.com", email);
    }

    [Fact]
    public void GenerateAccessToken_TokenContainsRole()
    {
        var user = CreateUser(role: UserRole.Admin);

        var token = _sut.GenerateAccessToken(user);
        var parsed = ParseToken(token);

        // ClaimTypes.Role маппится на стандартный role claim
        var roleClaim = parsed.Claims.First(c =>
            c.Type == ClaimTypes.Role ||
            c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
        Assert.Equal("Admin", roleClaim.Value);
    }

    [Fact]
    public void GenerateAccessToken_ExpiresInApproximately15Minutes()
    {
        var user = CreateUser();

        var token = _sut.GenerateAccessToken(user);
        var parsed = ParseToken(token);

        // Проверяем, что срок действия ≈ 15 минут
        var expectedExpiry = DateTime.UtcNow.AddMinutes(15);
        Assert.True(
            parsed.ValidTo >= expectedExpiry.AddSeconds(-5) &&
            parsed.ValidTo <= expectedExpiry.AddSeconds(5),
            $"Ожидалось ValidTo ≈ {expectedExpiry:u}, фактически: {parsed.ValidTo:u}");
    }

    [Fact]
    public void GenerateAccessToken_TokenHasCorrectIssuerAndAudience()
    {
        var user = CreateUser();

        var token = _sut.GenerateAccessToken(user);
        var parsed = ParseToken(token);

        Assert.Equal(TestIssuer,   parsed.Issuer);
        Assert.Contains(TestAudience, parsed.Audiences);
    }

    // ─────────────────────── ValidateAccessToken ────────────────────────────

    [Fact]
    public void ValidateAccessToken_ValidToken_ReturnsCorrectUserId()
    {
        var user = CreateUser(id: 99);
        var token = _sut.GenerateAccessToken(user);

        var userId = _sut.ValidateAccessToken(token);

        Assert.Equal(99, userId);
    }

    [Fact]
    public void ValidateAccessToken_GarbageString_ReturnsNull()
    {
        var userId = _sut.ValidateAccessToken("not.a.jwt.token");

        Assert.Null(userId);
    }

    [Fact]
    public void ValidateAccessToken_TokenSignedWithWrongKey_ReturnsNull()
    {
        // Создаём токен с другим ключом
        var wrongKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes("wrong-key-that-is-also-32-chars-long!!!"));
        var credentials = new SigningCredentials(wrongKey, SecurityAlgorithms.HmacSha256);

        var jwtToken = new JwtSecurityToken(
            issuer: TestIssuer,
            audience: TestAudience,
            claims: [new Claim(JwtRegisteredClaimNames.Sub, "1")],
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(jwtToken);

        var userId = _sut.ValidateAccessToken(tokenString);

        Assert.Null(userId);
    }

    [Fact]
    public void ValidateAccessToken_ExpiredToken_ReturnsNull()
    {
        // Генерируем токен с истёкшим сроком вручную — через тот же ключ, но с прошедшей датой
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwtToken = new JwtSecurityToken(
            issuer: TestIssuer,
            audience: TestAudience,
            claims: [new Claim(JwtRegisteredClaimNames.Sub, "1")],
            expires: DateTime.UtcNow.AddMinutes(-5), // уже истёк
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(jwtToken);

        var userId = _sut.ValidateAccessToken(tokenString);

        Assert.Null(userId);
    }

    // ─────────────────────── GenerateRefreshToken ───────────────────────────

    [Fact]
    public void GenerateRefreshToken_ReturnsNonEmptyBase64String()
    {
        var token = _sut.GenerateRefreshToken();

        Assert.NotNull(token);
        Assert.NotEmpty(token);

        // Убеждаемся, что строка — валидный Base64
        var bytes = Convert.FromBase64String(token);
        Assert.Equal(64, bytes.Length); // 64 random bytes
    }

    [Fact]
    public void GenerateRefreshToken_EachCallReturnsDifferentValue()
    {
        var token1 = _sut.GenerateRefreshToken();
        var token2 = _sut.GenerateRefreshToken();

        Assert.NotEqual(token1, token2);
    }

    // ─────────────────────── Helpers ────────────────────────────────────────

    // PasswordHash.Create требует BCrypt-формат: начинается с $2, длина ровно 60 символов
    private const string ValidBcryptHash = "$2a$11$AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";

    private static User CreateUser(
        int id = 1,
        string email = "user@test.com",
        UserRole role = UserRole.Client)
    {
        var user = User.Create(email, ValidBcryptHash, "Тест", "Тестов", "+77001234567", role);
        user.Id = id;
        return user;
    }

    private static JwtSecurityToken ParseToken(string token)
        => new JwtSecurityTokenHandler().ReadJwtToken(token);
}
