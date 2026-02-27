using Application.DTOs.FAQ;
using MediatR;

namespace Application.Features.FAQ.GetFAQs;

/// <summary>Получить список активных FAQ (публичный endpoint).</summary>
public record GetFAQsQuery : IRequest<IReadOnlyList<FaqResponse>>;
