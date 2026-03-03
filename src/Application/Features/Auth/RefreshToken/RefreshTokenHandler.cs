using Application.DTOs.Auth;
using Application.Interfaces;
using Application.Resources;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Application.Features.Auth.RefreshToken;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IStringLocalizer<ErrorMessages> _localizer;

    public RefreshTokenHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        IJwtService jwtService,
        IStringLocalizer<ErrorMessages> localizer)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _jwtService = jwtService;
        _localizer = localizer;
    }

    public async Task<RefreshTokenResponse> Handle(
        RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;

        // Найти refresh token в БД
        var refreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);
        if (refreshToken == null)
        {
            throw new UnauthorizedAccessException(_localizer["InvalidRefreshToken"]);
        }

        // Проверить валидность токена
        if (!refreshToken.IsValid())
        {
            throw new UnauthorizedAccessException(_localizer["RefreshTokenExpired"]);
        }

        // Получить пользователя
        var user = await _userRepository.GetByIdAsync(refreshToken.UserId);
        if (user == null || !user.IsActive)
        {
            throw new UnauthorizedAccessException(_localizer["UserNotFoundOrDeactivated"]);
        }

        // Сгенерировать новые токены
        var newAccessToken = _jwtService.GenerateAccessToken(user);
        var newRefreshTokenString = _jwtService.GenerateRefreshToken();

        // Отозвать старый refresh token и заменить новым
        refreshToken.Revoke(replacedByToken: newRefreshTokenString);
        await _refreshTokenRepository.UpdateAsync(refreshToken);

        // Сохранить новый refresh token
        var newRefreshToken = Domain.Entities.RefreshToken.Create(
            userId: user.Id,
            token: newRefreshTokenString,
            expiresAt: DateTime.UtcNow.AddDays(30)
        );
        await _refreshTokenRepository.AddAsync(newRefreshToken);

        return new RefreshTokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshTokenString,
            ExpiresIn = 900 // 15 минут
        };
    }
}