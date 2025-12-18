namespace Application.Interfaces;

public interface IPasswordHasher //Интерфейс для хэширования паролей
{
    string HashPassword(string password); // Принимает пароль в открытом в виде и возвращает в хэш
    bool VerifyPassword(string password, string hash); // Проверяем совпадает ли введённый пароль с хэшем
}