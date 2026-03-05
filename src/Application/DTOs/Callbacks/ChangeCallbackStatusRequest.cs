using Domain.Enums;

namespace Application.DTOs.Callbacks;

/// <summary>
/// Запрос на смену статуса заявки администратором.
/// </summary>
public class ChangeCallbackStatusRequest
{
    /// <summary>Целевой статус: Processed или Rejected.</summary>
    public CallbackRequestStatus Status { get; init; }
}
