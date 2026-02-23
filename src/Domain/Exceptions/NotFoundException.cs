namespace Domain.Exceptions;

public class NotFoundException : Exception
{
    public string EntityName { get; }
    public object EntityId { get; }

    public NotFoundException(string entityName, object entityId)
        : base($"{entityName} с ID '{entityId}' не найден.")
    {
        EntityName = entityName;
        EntityId = entityId;
    }
}