using MediatR;

namespace Application.UseCases.EventUseCases.GetEventByPlace;

public sealed record GetEventByPlaceRequest(string name): IRequest<GetEventByPlaceResponse>
{
    
}