using MediatR;

namespace Application.UseCases.EventUseCases.UnRegisterUserOnEvent;

public sealed record UnRegisterUserOnEventRequest: IRequest<UnRegisterUserOnEventResponse>
{
    public string userId { get; set; }
    public string eventId { get; set; }
}