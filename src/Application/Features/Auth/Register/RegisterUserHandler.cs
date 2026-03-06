using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Auth.Register;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, RegisterResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<RegisterResponse> Handle(
        RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;

        // Проверить, что email не занят
        if (await _userRepository.EmailExistsAsync(request.Email))
        {
            throw new BusinessRuleException($"Email '{request.Email}' уже зарегистрирован.");
        }

        // Хэшировать пароль
        var passwordHash = _passwordHasher.HashPassword(request.Password);

        // Создать пользователя через фабричный метод
        var user = User.Create(
            email: request.Email,
            passwordHash: passwordHash,
            firstName: request.FirstName,
            lastName: request.LastName,
            phone: request.Phone,
            role: UserRole.Client,
            city: request.City
        );

        // Сохранить в БД
        await _userRepository.AddAsync(user);

        // Вернуть результат
        return new RegisterResponse
        {
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName
        };
    }
}