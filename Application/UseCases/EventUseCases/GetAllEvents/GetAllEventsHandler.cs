using Domain.Repositories.Interfaces;
using MediatR;

namespace Application.UseCases.EventUseCases.GetAllEvents;

public class GetAllEventsHandler: IRequestHandler<GetAllEventsRequest, GetAllEventsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllEventsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<GetAllEventsResponse> Handle(GetAllEventsRequest request, CancellationToken cancellationToken)
    {
        return new GetAllEventsResponse()
        {
            Events = await _unitOfWork.EventRepository.GetAllAsync(cancellationToken)
        };
    }
}