using MediatR;

namespace Application.UseCases.EventUseCases.GetEventById;

public sealed record GetEventByIdRequest(string id): IRequest<GetEventByIdResponse>
{
    
}