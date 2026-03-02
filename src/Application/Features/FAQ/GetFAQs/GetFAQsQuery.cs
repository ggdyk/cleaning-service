using Application.DTOs.FAQ;
using MediatR;

namespace Application.Features.FAQ.GetFAQs;

/// <summary>Получить список активных FAQ на языке текущего запроса.</summary>
public record GetFAQsQuery : IRequest<IReadOnlyList<FaqLocalizedResponse>>;
