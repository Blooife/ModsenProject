using Domain.Repositories.Interfaces;
using MediatR;

namespace Application.UseCases.EventUseCases.GetPagedEvents;

public class GetPagedEventsHandler: IRequestHandler<GetPagedEventsRequest, GetPagedEventsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPagedEventsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<GetPagedEventsResponse> Handle(GetPagedEventsRequest request, CancellationToken cancellationToken)
    {
        var res =  await _unitOfWork.EventRepository.GetPagedEventsAsync(request.PageNumber, request.PageSize,
            cancellationToken);
        return new GetPagedEventsResponse()
        {
            events = res,
        };
    }
}