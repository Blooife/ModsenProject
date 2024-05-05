using Application.Exceptions;
using AutoMapper;
using Domain.Repositories.Interfaces;
using MediatR;

namespace Application.UseCases.EventUseCases.GetEventByPlace;

public class GetEventByPlaceHandler: IRequestHandler<GetEventByPlaceRequest, GetEventByPlaceResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetEventByPlaceHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<GetEventByPlaceResponse> Handle(GetEventByPlaceRequest request, CancellationToken cancellationToken)
    {
        var ev =  await _unitOfWork.EventRepository.GetByPlaceAsync(request.name, cancellationToken);
        if (ev == null)
        {
            throw new NotFoundException("Event", request.name);
        }
        return new GetEventByPlaceResponse()
        {
            events = ev,
        };
    }
}