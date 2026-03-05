using Domain.Entities;

namespace UnitTests.Auth;

public class RefreshTokenEntityTests
{
    // ─────────────────────── Create ─────────────────────────────────────────

    [Fact]
    public void Create_ValidParams_CreatesToken()
    {
        var token = RefreshToken.Create(
            userId: 1,
            token: "valid-refresh-token",
            expiresAt: DateTime.UtcNow.AddDays(30));

        Assert.Equal(1, token.UserId);
        Assert.Equal("valid-refresh-token", token.Token);
        Assert.False(token.IsRevoked);
        Assert.True(token.ExpiresAt > DateTime.UtcNow);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Create_InvalidUserId_ThrowsArgumentException(int userId)
    {
        Assert.Throws<ArgumentException>(() =>
            RefreshToken.Create(userId, "token", DateTime.UtcNow.AddDays(1)));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyToken_ThrowsArgumentException(string emptyToken)
    {
        Assert.Throws<ArgumentException>(() =>
            RefreshToken.Create(1, emptyToken, DateTime.UtcNow.AddDays(1)));
    }

    [Fact]
    public void Create_PastExpiresAt_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            RefreshToken.Create(1, "token", DateTime.UtcNow.AddMinutes(-1)));
    }

    [Fact]
    public void Create_CurrentMomentExpiresAt_ThrowsArgumentException()
    {
        // expiresAt должен быть СТРОГО в будущем, не равен UtcNow
        Assert.Throws<ArgumentException>(() =>
            RefreshToken.Create(1, "token", DateTime.UtcNow));
    }

    // ─────────────────────── IsValid ────────────────────────────────────────

    [Fact]
    public void IsValid_FreshToken_ReturnsTrue()
    {
        var token = CreateValidToken();

        Assert.True(token.IsValid());
    }

    [Fact]
    public void IsValid_RevokedToken_ReturnsFalse()
    {
        var token = CreateValidToken();
        token.Revoke();

        Assert.False(token.IsValid());
    }

    // ─────────────────────── IsExpired ──────────────────────────────────────

    [Fact]
    public void IsExpired_FutureExpiresAt_ReturnsFalse()
    {
        var token = CreateValidToken(expiresInDays: 30);

        Assert.False(token.IsExpired());
    }

    // ─────────────────────── Revoke ─────────────────────────────────────────

    [Fact]
    public void Revoke_SetsIsRevokedAndRevokedAt()
    {
        var token = CreateValidToken();
        var beforeRevoke = DateTime.UtcNow;

        token.Revoke();

        Assert.True(token.IsRevoked);
        Assert.NotNull(token.RevokedAt);
        Assert.True(token.RevokedAt >= beforeRevoke);
    }

    [Fact]
    public void Revoke_WithReplacedByToken_SetsReplacedByToken()
    {
        var token = CreateValidToken();

        token.Revoke(replacedByToken: "new-refresh-token-abc");

        Assert.Equal("new-refresh-token-abc", token.ReplacedByToken);
    }

    [Fact]
    public void Revoke_WithoutReplacedByToken_ReplacedByTokenIsNull()
    {
        var token = CreateValidToken();

        token.Revoke();

        Assert.Null(token.ReplacedByToken);
    }

    [Fact]
    public void Revoke_CalledTwice_StaysRevoked()
    {
        var token = CreateValidToken();

        token.Revoke("first");
        token.Revoke("second"); // допускается повторный отзыв

        Assert.True(token.IsRevoked);
    }

    // ─────────────────────── Helpers ────────────────────────────────────────

    private static RefreshToken CreateValidToken(int expiresInDays = 30)
        => RefreshToken.Create(
            userId: 1,
            token: "some-valid-refresh-token-string",
            expiresAt: DateTime.UtcNow.AddDays(expiresInDays));
}
