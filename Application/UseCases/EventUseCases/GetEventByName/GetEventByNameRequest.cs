using MediatR;

namespace Application.UseCases.EventUseCases.GetEventByName;

public sealed record GetEventByNameRequest(string name): IRequest<GetEventByNameResponse>
{
    
}