using Application.DTOs.FAQ;
using MediatR;

namespace Application.Features.FAQ.Admin.GetAllFAQs;

/// <summary>Получить все FAQ (включая неактивные) — только для Admin/Manager.</summary>
public record GetAllFAQsQuery : IRequest<IReadOnlyList<FaqResponse>>;
