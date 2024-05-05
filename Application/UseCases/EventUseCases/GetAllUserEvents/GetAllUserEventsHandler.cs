using Domain.Repositories.Interfaces;
using MediatR;

namespace Application.UseCases.EventUseCases.GetAllUserEvents;

public class GetAllUserEventsHandler: IRequestHandler<GetAllUserEventsRequest, GetAllUserEventsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllUserEventsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<GetAllUserEventsResponse> Handle(GetAllUserEventsRequest request, CancellationToken cancellationToken)
    {
        var res = await _unitOfWork.EventsUsersRepository.GetAllUserEvents(request.id, cancellationToken);
        return new GetAllUserEventsResponse()
        {
            events = res,
        };
    }
}