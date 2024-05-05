using Application.Exceptions;
using AutoMapper;
using Domain.Repositories.Interfaces;
using MediatR;

namespace Application.UseCases.EventUseCases.GetEventByDate;

public class GetEventByDateHandler: IRequestHandler<GetEventByDateRequest, GetEventByDateResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetEventByDateHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<GetEventByDateResponse> Handle(GetEventByDateRequest request, CancellationToken cancellationToken)
    {
        var ev =  await _unitOfWork.EventRepository.GetByDateAsync(request.date, cancellationToken);
        if (ev == null)
        {
            throw new NotFoundException("Event", request.date);
        }
        return new GetEventByDateResponse()
        {
            events = ev,
        };
    }
}