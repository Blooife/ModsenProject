using MediatR;

namespace Application.UseCases.EventUseCases.GetAllEvents;

public sealed record GetAllEventsRequest: IRequest<GetAllEventsResponse>
{
    
}