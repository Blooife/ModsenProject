using Application.Exceptions;
using Domain.Repositories.Interfaces;
using MediatR;

namespace Application.UseCases.EventUseCases.RegisterUserOnEvent;

public class RegisterUserOnEventHandler: IRequestHandler<RegisterUserOnEventRequest, RegisterUserOnEventResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserOnEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<RegisterUserOnEventResponse> Handle(RegisterUserOnEventRequest request, CancellationToken cancellationToken)
    {
        var ev = await _unitOfWork.EventRepository.GetByIdAsync(request.eventId, cancellationToken);
        if (ev == null)
        {
            throw new NotFoundException("Event", request.eventId);
        }

        if (ev.PlacesLeft < 1)
        {
            throw new ArgumentException("No places left");
        }
        if (!await _unitOfWork.UserRepository.Exists(request.userId, cancellationToken))
        {
            throw new NotFoundException("User", request.userId);
        }
        await _unitOfWork.EventsUsersRepository.RegisterUserOnEvent(request.userId, request.eventId, cancellationToken);
        await _unitOfWork.EventRepository.UpdatePlacesLeftAsync(ev, 1, cancellationToken);
        await _unitOfWork.Save();
        return new RegisterUserOnEventResponse()
        {
            Message = "User successfully registered on event",
        };
    }
}