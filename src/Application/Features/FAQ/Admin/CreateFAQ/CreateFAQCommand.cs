using Application.DTOs.FAQ;
using MediatR;

namespace Application.Features.FAQ.Admin.CreateFAQ;

public record CreateFAQCommand(CreateFaqRequest Request) : IRequest<FaqResponse>;
