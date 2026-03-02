using Application.DTOs.Callbacks;
using MediatR;
using SubmitCallbackRequestDto = Application.DTOs.Callbacks.SubmitCallbackRequest;

namespace Application.Features.Callbacks.SubmitCallbackRequest;

public record SubmitCallbackRequestCommand(SubmitCallbackRequestDto Request) : IRequest<CallbackRequestResponse>;
