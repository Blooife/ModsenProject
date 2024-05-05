using Application.Exceptions;
using Domain.Repositories.Interfaces;
using MediatR;

namespace Application.UseCases.EventUseCases.UnRegisterUserOnEvent;

public class UnRegisterUserOnEventHandler: IRequestHandler<UnRegisterUserOnEventRequest, UnRegisterUserOnEventResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public UnRegisterUserOnEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<UnRegisterUserOnEventResponse> Handle(UnRegisterUserOnEventRequest request, CancellationToken cancellationToken)
    {
        var ev = await _unitOfWork.EventRepository.GetByIdAsync(request.eventId, cancellationToken);
        if (ev == null)
        {
            throw new NotFoundException("Event", request.eventId);
        }
        if (!await _unitOfWork.UserRepository.Exists(request.userId, cancellationToken))
        {
            throw new NotFoundException("User", request.userId);
        }
        var unregisterResult = await _unitOfWork.EventsUsersRepository.UnregisterUserOnEvent(request.userId, request.eventId, cancellationToken);
        if (!unregisterResult)
        {
            throw new NotFoundException("users events for ", request.userId);
        }
        await _unitOfWork.EventRepository.UpdatePlacesLeftAsync(ev, -1, cancellationToken);
        
        await _unitOfWork.Save();
        return new UnRegisterUserOnEventResponse()
        {
            Message = "User successfully unregistered on event",
        };
    }
}