using MediatR;

namespace Application.UseCases.EventUseCases.DeleteEvent;

public sealed record DeleteEventRequest(string id): IRequest<DeleteEventResponse>
{
    
}