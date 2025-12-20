using Application.DTOs.Auth;
using MediatR;

namespace Application.Features.Auth.Login;

public record LoginCommand(LoginRequest Request) : IRequest<LoginResponse>;