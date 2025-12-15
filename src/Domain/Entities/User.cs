using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities;

public class User : BaseEntity
{
    // Private setters для инкапсуляции
    public Email Email { get; private set; }
    public PasswordHash PasswordHash { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Phone { get; private set; }
    public UserRole Role { get; private set; }
    public string? City { get; private set; }
    public DateTime RegisteredAt { get; private set; }
    public bool IsActive { get; private set; }

    // Конструктор для EF Core (private)
    private User()
    {
        Email = null!;
        PasswordHash = null!;
        FirstName = null!;
        LastName = null!;
        Phone = null!;
    }

    // Фабричный метод для создания нового пользователя
    public static User Create(
        string email,
        string passwordHash,
        string firstName,
        string lastName,
        string phone,
        UserRole role = UserRole.Client,
        string? city = null)
    {
        // Валидация
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Phone cannot be empty", nameof(phone));

        if (firstName.Length > 100)
            throw new ArgumentException("First name cannot exceed 100 characters", nameof(firstName));

        if (lastName.Length > 100)
            throw new ArgumentException("Last name cannot exceed 100 characters", nameof(lastName));

        // Простая валидация телефона (можно улучшить)
        if (phone.Length < 10 || phone.Length > 20)
            throw new ArgumentException("Phone must be between 10 and 20 characters", nameof(phone));

        return new User
        {
            Email = Email.Create(email),
            PasswordHash = PasswordHash.Create(passwordHash),
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Phone = phone.Trim(),
            Role = role,
            City = city?.Trim(),
            RegisteredAt = DateTime.UtcNow,
            IsActive = true
        };
    }

    // Методы поведения (behavior)
    public void UpdateProfile(string firstName, string lastName, string phone, string? city = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Phone cannot be empty", nameof(phone));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Phone = phone.Trim();
        City = city?.Trim();
    }

    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = PasswordHash.Create(newPasswordHash);
    }

    public void ChangeRole(UserRole newRole)
    {
        Role = newRole;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public string GetFullName() => $"{FirstName} {LastName}";
}