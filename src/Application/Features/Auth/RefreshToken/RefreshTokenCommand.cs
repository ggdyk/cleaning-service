using Application.DTOs.Auth;
using MediatR;

namespace Application.Features.Auth.RefreshToken;

public record RefreshTokenCommand(RefreshTokenRequest Request) : IRequest<RefreshTokenResponse>;