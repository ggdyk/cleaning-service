namespace Domain.Exceptions;

public class ForbiddenException : Exception
{
    public ForbiddenException(string message = "У вас нет прав для выполнения этой операции.")
        : base(message) { }
}