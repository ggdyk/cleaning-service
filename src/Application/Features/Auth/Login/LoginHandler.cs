using Application.DTOs.Auth;
using Application.Interfaces;
using Application.Resources;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Application.Features.Auth.Login;

public class LoginHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IStringLocalizer<ErrorMessages> _localizer;

    public LoginHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        IRefreshTokenRepository refreshTokenRepository,
        IStringLocalizer<ErrorMessages> localizer)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _refreshTokenRepository = refreshTokenRepository;
        _localizer = localizer;
    }

    public async Task<LoginResponse> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        // Найти пользователя по email
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
        {
            throw new UnauthorizedAccessException(_localizer["InvalidCredentials"]);
        }

        // Проверить пароль
        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException(_localizer["InvalidCredentials"]);
        }

        // Проверить, что пользователь активен
        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException(_localizer["AccountDeactivated"]);
        }

        // Отозвать старые refresh токены
        await _refreshTokenRepository.RevokeAllByUserIdAsync(user.Id);

        // Сгенерировать новые токены
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshTokenString = _jwtService.GenerateRefreshToken();

        // Сохранить refresh token в БД
        var refreshToken = Domain.Entities.RefreshToken.Create(
            userId: user.Id,
            token: refreshTokenString,
            expiresAt: DateTime.UtcNow.AddDays(30)
        );
        await _refreshTokenRepository.AddAsync(refreshToken);

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenString,
            ExpiresIn = 900, // 15 минут
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString()
            }
        };
    }
}