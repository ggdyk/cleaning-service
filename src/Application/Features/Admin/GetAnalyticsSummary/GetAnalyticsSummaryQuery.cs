using Application.DTOs.Admin;
using MediatR;

namespace Application.Features.Admin.GetAnalyticsSummary;

public record GetAnalyticsSummaryQuery : IRequest<AnalyticsSummaryDto>;
