using MediatR;

namespace Application.UseCases.EventUseCases.GetAllUserEvents;

public sealed record GetAllUserEventsRequest(string id): IRequest<GetAllUserEventsResponse>
{
    
}