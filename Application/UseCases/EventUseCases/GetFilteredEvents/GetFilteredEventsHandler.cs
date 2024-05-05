using Domain.Models.Entities;
using Domain.Repositories.Interfaces;
using MediatR;

namespace Application.UseCases.EventUseCases.GetFilteredEvents;

public class GetFilteredEventsHandler: IRequestHandler<GetFilteredEventsRequest, GetFilteredEventsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetFilteredEventsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<GetFilteredEventsResponse> Handle(GetFilteredEventsRequest request, CancellationToken cancellationToken)
    {
        IEnumerable<Event> res = Enumerable.Empty<Event>();
        IEnumerable<Event> res1 = Enumerable.Empty<Event>();
        if (!string.IsNullOrEmpty(request.category) && !string.IsNullOrEmpty(request.place))
        {
            res = await _unitOfWork.EventRepository.GetByCategoryAsync(request.category, cancellationToken);
            res1 = await _unitOfWork.EventRepository.GetByPlaceAsync(request.place, cancellationToken);
            return new GetFilteredEventsResponse()
            {
                events = res.Intersect(res1),
            };
        }
        if(!string.IsNullOrEmpty(request.category))
        {
            return new GetFilteredEventsResponse()
            {
                events = await _unitOfWork.EventRepository.GetByCategoryAsync(request.category, cancellationToken),
            };
            
        }
        if(!string.IsNullOrEmpty(request.place))
        {
            return new GetFilteredEventsResponse()
            {
                events = await _unitOfWork.EventRepository.GetByPlaceAsync(request.place, cancellationToken),
            };
        }
        return new GetFilteredEventsResponse()
        {
            events = res,
        };
    }
}