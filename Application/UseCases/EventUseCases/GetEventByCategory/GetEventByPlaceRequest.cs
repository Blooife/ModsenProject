using MediatR;

namespace Application.UseCases.EventUseCases.GetEventByCategory;

public sealed record GetEventByCategoryRequest(string category): IRequest<GetEventByCategoryResponse>
{
    
}