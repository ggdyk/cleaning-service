using Domain.Common;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities;

/// <summary>
/// Заявка на обратный звонок, оставленная посетителем сайта.
/// </summary>
public class CallBackRequest : BaseEntity
{
    /// <summary>Имя клиента.</summary>
    public string Name { get; private set; } = default!;

    /// <summary>Контактный номер телефона.</summary>
    public string Phone { get; private set; } = default!;

    /// <summary>Удобное время для звонка (свободный текст: "с 10 до 12", "вечером" и т.д.).</summary>
    public string? PreferredTime { get; private set; }

    /// <summary>Статус обработки заявки.</summary>
    public CallbackRequestStatus Status { get; private set; }

    /// <summary>Дата и время создания заявки (UTC).</summary>
    public DateTime CreatedAt { get; private set; }

    // Конструктор для EF Core
    private CallBackRequest() { }

    /// <summary>
    /// Создать новую заявку на обратный звонок.
    /// </summary>
    /// <param name="name">Имя клиента.</param>
    /// <param name="phone">Номер телефона.</param>
    /// <param name="preferredTime">Удобное время (необязательно).</param>
    public static CallBackRequest Create(string name, string phone, string? preferredTime = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Имя не может быть пустым.", nameof(name));

        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Телефон не может быть пустым.", nameof(phone));

        return new CallBackRequest
        {
            Name = name.Trim(),
            Phone = phone.Trim(),
            PreferredTime = preferredTime?.Trim(),
            Status = CallbackRequestStatus.New,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Отметить заявку как обработанную (оператор позвонил клиенту).
    /// Допустимо только из статуса New.
    /// </summary>
    public void Process()
    {
        if (Status != CallbackRequestStatus.New)
            throw new BusinessRuleException(
                $"Нельзя обработать заявку — она в статусе '{Status}'. Ожидается: New.");

        Status = CallbackRequestStatus.Processed;
    }

    /// <summary>
    /// Отклонить заявку (неверный номер, отказ клиента и т.д.).
    /// Допустимо только из статуса New.
    /// </summary>
    public void Reject()
    {
        if (Status != CallbackRequestStatus.New)
            throw new BusinessRuleException(
                $"Нельзя отклонить заявку — она в статусе '{Status}'. Ожидается: New.");

        Status = CallbackRequestStatus.Rejected;
    }
}
