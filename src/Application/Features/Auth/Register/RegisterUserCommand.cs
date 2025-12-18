using Application.DTOs.Auth;
using MediatR;

namespace Application.Features.Auth.Register;

public record RegisterUserCommand(RegisterRequest Request) : IRequest<RegisterResponse>;