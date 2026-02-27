using MediatR;

namespace Application.Features.FAQ.Admin.DeleteFAQ;

public record DeleteFAQCommand(int Id) : IRequest;
