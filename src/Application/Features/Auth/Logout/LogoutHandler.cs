using Application.Interfaces;
using MediatR;

namespace Application.Features.Auth.Logout;

public class LogoutHandler : IRequestHandler<LogoutCommand>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LogoutHandler(IRefreshTokenRepository refreshTokenRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        // Отозвать все refresh токены пользователя
        await _refreshTokenRepository.RevokeAllByUserIdAsync(command.UserId);
    }
}