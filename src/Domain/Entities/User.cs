namespace Domain.Entities;

public class User
{
    public Guid Id { get; } // уникальный id
    public string Email { get; set; } = default!; // узнать что это?
    public string PasswordHash { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string Role { get; set; } = "client"; // клиент или админ. как вариант сделать enum???
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public string City { get; set; } //для будущего расширения
}