namespace Domain.Entities;

public class User
{
    public Guid Id { get; } // уникальный id пользователя
    public string Email { get; set; } = default!; // инициализация что бы подавить предупреждение, компилятор знает
                                                  // что потом будет значение
    public string PasswordHash { get; set; } = default!; // хеш пароля, а не сам пароль
    public string FirstName { get; set; } = default!; 
    public string LastName { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string Role { get; set; } = "client"; // клиент или админ. как вариант сделать enum???
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow; // дата регистрация 
    public bool IsActive { get; set; } = true;
    public string City { get; set; } //для будущего расширения
}