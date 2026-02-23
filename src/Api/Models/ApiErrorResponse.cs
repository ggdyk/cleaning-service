namespace Api.Models;

public sealed class ApiErrorResponse
{
    public int StatusCode { get; init; }
    public string Error { get; init; } = default!;
    public string Message { get; init; } = default!;
    public IDictionary<string, string[]>? ValidationErrors { get; init; }
    public string TraceId { get; init; } = default!;
}