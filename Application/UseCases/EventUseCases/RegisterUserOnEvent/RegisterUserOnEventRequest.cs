using MediatR;

namespace Application.UseCases.EventUseCases.RegisterUserOnEvent;

public sealed record RegisterUserOnEventRequest: IRequest<RegisterUserOnEventResponse>
{
    public string userId { get; set; }
    public string eventId { get; set; }
}