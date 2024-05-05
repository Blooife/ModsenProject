using Application.Exceptions;
using Domain.Repositories.Interfaces;
using MediatR;

namespace Application.UseCases.EventUseCases.GetEventByCategory;

public class GetEventByCategoryHandler: IRequestHandler<GetEventByCategoryRequest, GetEventByCategoryResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetEventByCategoryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<GetEventByCategoryResponse> Handle(GetEventByCategoryRequest request, CancellationToken cancellationToken)
    {
        var ev =  await _unitOfWork.EventRepository.GetByCategoryAsync(request.category, cancellationToken);
        if (ev == null)
        {
            throw new NotFoundException("Event", request.category);
        }
        return new GetEventByCategoryResponse()
        {
            events = ev,
        };
    }
}