using Application.DTOs.Content;
using MediatR;

namespace Application.Features.Content.GetAboutPage;

public record GetAboutPageQuery : IRequest<PageResponse>;
