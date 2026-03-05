using Application.DTOs.Auth;
using Application.Features.Auth.RefreshToken;
using Application.Interfaces;
using Application.Resources;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Localization;

namespace UnitTests.Auth;

public class RefreshTokenHandlerTests
{
    private readonly Mock<IRefreshTokenRepository> _tokenRepo  = new();
    private readonly Mock<IUserRepository>         _userRepo   = new();
    private readonly Mock<IJwtService>              _jwtService = new();
    private readonly Mock<IStringLocalizer<ErrorMessages>> _localizer = new();

    private readonly RefreshTokenHandler _sut;

    public RefreshTokenHandlerTests()
    {
        _localizer.Setup(l => l[It.IsAny<string>()])
            .Returns<string>(key => new LocalizedString(key, key));

        _sut = new RefreshTokenHandler(
            _tokenRepo.Object,
            _userRepo.Object,
            _jwtService.Object,
            _localizer.Object);
    }

    // ─────────────────────── Happy path ─────────────────────────────────────

    [Fact]
    public async Task Handle_ValidRefreshToken_ReturnsNewTokenPair()
    {
        var user         = CreateActiveUser();
        var refreshToken = CreateValidRefreshToken(user.Id);

        _tokenRepo.Setup(r => r.GetByTokenAsync("old-refresh")).ReturnsAsync(refreshToken);
        _userRepo.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
        _jwtService.Setup(j => j.GenerateAccessToken(user)).Returns("new-access");
        _jwtService.Setup(j => j.GenerateRefreshToken()).Returns("new-refresh");
        _tokenRepo.Setup(r => r.UpdateAsync(refreshToken)).Returns(Task.CompletedTask);
        _tokenRepo.Setup(r => r.AddAsync(It.IsAny<RefreshToken>())).Returns(Task.CompletedTask);

        var result = await _sut.Handle(
            new RefreshTokenCommand(new RefreshTokenRequest { RefreshToken = "old-refresh" }),
            CancellationToken.None);

        Assert.Equal("new-access",  result.AccessToken);
        Assert.Equal("new-refresh", result.RefreshToken);
        Assert.Equal(900,           result.ExpiresIn);
    }

    // ─────────────────────── Ошибки ─────────────────────────────────────────

    [Fact]
    public async Task Handle_UnknownRefreshToken_ThrowsUnauthorizedAccessException()
    {
        _tokenRepo.Setup(r => r.GetByTokenAsync(It.IsAny<string>()))
            .ReturnsAsync((RefreshToken?)null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _sut.Handle(
                new RefreshTokenCommand(new RefreshTokenRequest { RefreshToken = "unknown" }),
                CancellationToken.None));
    }

    [Fact]
    public async Task Handle_RevokedRefreshToken_ThrowsUnauthorizedAccessException()
    {
        var refreshToken = CreateValidRefreshToken(userId: 1);
        refreshToken.Revoke(); // токен отозван → IsValid() = false

        _tokenRepo.Setup(r => r.GetByTokenAsync("revoked-token")).ReturnsAsync(refreshToken);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _sut.Handle(
                new RefreshTokenCommand(new RefreshTokenRequest { RefreshToken = "revoked-token" }),
                CancellationToken.None));
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsUnauthorizedAccessException()
    {
        var refreshToken = CreateValidRefreshToken(userId: 999);

        _tokenRepo.Setup(r => r.GetByTokenAsync("token")).ReturnsAsync(refreshToken);
        _userRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _sut.Handle(
                new RefreshTokenCommand(new RefreshTokenRequest { RefreshToken = "token" }),
                CancellationToken.None));
    }

    [Fact]
    public async Task Handle_DeactivatedUser_ThrowsUnauthorizedAccessException()
    {
        var user         = CreateActiveUser();
        user.Deactivate();
        var refreshToken = CreateValidRefreshToken(user.Id);

        _tokenRepo.Setup(r => r.GetByTokenAsync("token")).ReturnsAsync(refreshToken);
        _userRepo.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _sut.Handle(
                new RefreshTokenCommand(new RefreshTokenRequest { RefreshToken = "token" }),
                CancellationToken.None));
    }

    // ─────────────────────── Взаимодействия ─────────────────────────────────

    [Fact]
    public async Task Handle_Success_RevokesOldTokenAndSavesNew()
    {
        var user         = CreateActiveUser();
        var refreshToken = CreateValidRefreshToken(user.Id, tokenValue: "old-refresh");

        _tokenRepo.Setup(r => r.GetByTokenAsync("old-refresh")).ReturnsAsync(refreshToken);
        _userRepo.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
        _jwtService.Setup(j => j.GenerateAccessToken(user)).Returns("new-access");
        _jwtService.Setup(j => j.GenerateRefreshToken()).Returns("new-refresh");
        _tokenRepo.Setup(r => r.UpdateAsync(refreshToken)).Returns(Task.CompletedTask);
        _tokenRepo.Setup(r => r.AddAsync(It.IsAny<RefreshToken>())).Returns(Task.CompletedTask);

        await _sut.Handle(
            new RefreshTokenCommand(new RefreshTokenRequest { RefreshToken = "old-refresh" }),
            CancellationToken.None);

        // Старый отозван и обновлён в БД
        _tokenRepo.Verify(r => r.UpdateAsync(It.Is<RefreshToken>(t => t.IsRevoked)), Times.Once);
        // Новый сохранён
        _tokenRepo.Verify(r => r.AddAsync(It.IsAny<RefreshToken>()), Times.Once);
    }

    // ─────────────────────── Helpers ────────────────────────────────────────

    // PasswordHash.Create требует BCrypt-формат: начинается с $2, длина ровно 60 символов
    private const string ValidBcryptHash = "$2a$11$AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";

    private static User CreateActiveUser()
    {
        var user = User.Create(
            "user@test.com", ValidBcryptHash, "Тест", "Тестов", "+77001234567", UserRole.Client);
        user.Id = 1;
        return user;
    }

    private static RefreshToken CreateValidRefreshToken(
        int userId,
        string tokenValue = "old-refresh")
        => RefreshToken.Create(userId, tokenValue, DateTime.UtcNow.AddDays(30));
}
