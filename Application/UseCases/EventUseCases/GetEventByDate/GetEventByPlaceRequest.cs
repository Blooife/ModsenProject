using MediatR;

namespace Application.UseCases.EventUseCases.GetEventByDate;

public sealed record GetEventByDateRequest(DateTime date): IRequest<GetEventByDateResponse>
{
    
}