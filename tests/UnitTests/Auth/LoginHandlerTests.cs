using Application.DTOs.Auth;
using Application.Features.Auth.Login;
using Application.Interfaces;
using Application.Resources;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Localization;

namespace UnitTests.Auth;

public class LoginHandlerTests
{
    private readonly Mock<IUserRepository>         _userRepo     = new();
    private readonly Mock<IPasswordHasher>          _hasher       = new();
    private readonly Mock<IJwtService>              _jwtService   = new();
    private readonly Mock<IRefreshTokenRepository> _tokenRepo    = new();
    private readonly Mock<IStringLocalizer<ErrorMessages>> _localizer = new();

    private readonly LoginHandler _sut;

    public LoginHandlerTests()
    {
        // Localizer: любой ключ → LocalizedString с тем же значением
        _localizer.Setup(l => l[It.IsAny<string>()])
            .Returns<string>(key => new LocalizedString(key, key));

        _sut = new LoginHandler(
            _userRepo.Object,
            _hasher.Object,
            _jwtService.Object,
            _tokenRepo.Object,
            _localizer.Object);
    }

    // ─────────────────────── Happy path ─────────────────────────────────────

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsLoginResponse()
    {
        var user = CreateActiveUser();
        _userRepo.Setup(r => r.GetByEmailAsync("user@test.com")).ReturnsAsync(user);
        _hasher.Setup(h => h.VerifyPassword("SecretPass!", user.PasswordHash)).Returns(true);
        _jwtService.Setup(j => j.GenerateAccessToken(user)).Returns("access.token");
        _jwtService.Setup(j => j.GenerateRefreshToken()).Returns("refresh-token");
        _tokenRepo.Setup(r => r.AddAsync(It.IsAny<RefreshToken>())).Returns(Task.CompletedTask);
        _tokenRepo.Setup(r => r.RevokeAllByUserIdAsync(user.Id)).Returns(Task.CompletedTask);

        var result = await _sut.Handle(
            new LoginCommand(new LoginRequest { Email = "user@test.com", Password = "SecretPass!" }),
            CancellationToken.None);

        Assert.Equal("access.token",  result.AccessToken);
        Assert.Equal("refresh-token", result.RefreshToken);
        Assert.Equal(900,             result.ExpiresIn);
        Assert.Equal("user@test.com", result.User.Email);
    }

    // ─────────────────────── Ошибки аутентификации ──────────────────────────

    [Fact]
    public async Task Handle_UnknownEmail_ThrowsUnauthorizedAccessException()
    {
        _userRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _sut.Handle(
                new LoginCommand(new LoginRequest { Email = "nobody@test.com", Password = "pass" }),
                CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WrongPassword_ThrowsUnauthorizedAccessException()
    {
        var user = CreateActiveUser();
        _userRepo.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
        _hasher.Setup(h => h.VerifyPassword("WrongPassword", It.IsAny<string>())).Returns(false);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _sut.Handle(
                new LoginCommand(new LoginRequest { Email = user.Email, Password = "WrongPassword" }),
                CancellationToken.None));
    }

    [Fact]
    public async Task Handle_DeactivatedUser_ThrowsUnauthorizedAccessException()
    {
        var user = CreateActiveUser();
        user.Deactivate();

        _userRepo.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
        _hasher.Setup(h => h.VerifyPassword(It.IsAny<string>(), user.PasswordHash)).Returns(true);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _sut.Handle(
                new LoginCommand(new LoginRequest { Email = user.Email, Password = "anypass" }),
                CancellationToken.None));
    }

    // ─────────────────────── Взаимодействия с зависимостями ─────────────────

    [Fact]
    public async Task Handle_SuccessfulLogin_RevokesOldTokensAndSavesNew()
    {
        var user = CreateActiveUser();
        _userRepo.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
        _hasher.Setup(h => h.VerifyPassword(It.IsAny<string>(), user.PasswordHash)).Returns(true);
        _jwtService.Setup(j => j.GenerateAccessToken(user)).Returns("access");
        _jwtService.Setup(j => j.GenerateRefreshToken()).Returns("refresh");
        _tokenRepo.Setup(r => r.RevokeAllByUserIdAsync(user.Id)).Returns(Task.CompletedTask);
        _tokenRepo.Setup(r => r.AddAsync(It.IsAny<RefreshToken>())).Returns(Task.CompletedTask);

        await _sut.Handle(
            new LoginCommand(new LoginRequest { Email = user.Email, Password = "pass" }),
            CancellationToken.None);

        // Обязательно: старые токены отозваны, новый сохранён
        _tokenRepo.Verify(r => r.RevokeAllByUserIdAsync(user.Id), Times.Once);
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
}
