namespace Domain.ValueObjects;

public sealed class PasswordHash
{
    public string Value { get; }

    private PasswordHash(string value)
    {
        Value = value;
    }

    public static PasswordHash Create(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            throw new ArgumentException("Password hash cannot be empty", nameof(hash));
        }

        // Bcrypt hash всегда начинается с $2a$, $2b$, $2y$ и имеет длину 60 символов
        if (!hash.StartsWith("$2") || hash.Length != 60)
        {
            throw new ArgumentException("Invalid password hash format", nameof(hash));
        }

        return new PasswordHash(hash);
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
        {
            return false;
        }

        return Value == ((PasswordHash)obj).Value;
    }

    public override int GetHashCode() => Value.GetHashCode();

    public static implicit operator string(PasswordHash hash) => hash.Value;
}