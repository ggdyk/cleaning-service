using Application.DTOs.FAQ;
using MediatR;

namespace Application.Features.FAQ.Admin.UpdateFAQ;

public record UpdateFAQCommand(int Id, UpdateFaqRequest Request) : IRequest<FaqResponse>;
