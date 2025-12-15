using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public sealed class Email
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be empty", nameof(email));
        }

        email = email.Trim().ToLowerInvariant();

        if (!EmailRegex.IsMatch(email))
        {
            throw new ArgumentException("Invalid email format", nameof(email));
        }

        if (email.Length > 255)
        {
            throw new ArgumentException("Email cannot exceed 255 characters", nameof(email));
        }

        return new Email(email);
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
        {
            return false;
        }

        return Value == ((Email)obj).Value;
    }

    public override int GetHashCode() => Value.GetHashCode();

    public static implicit operator string(Email email) => email.Value;
}