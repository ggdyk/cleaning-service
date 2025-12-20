using MediatR;

namespace Application.Features.Auth.Logout;

public record LogoutCommand(int UserId) : IRequest;