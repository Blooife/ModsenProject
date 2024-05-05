using AutoMapper;
using Domain.Models.Entities;
using Domain.Repositories.Interfaces;
using MediatR;

namespace Application.UseCases.EventUseCases.CreateEvent;

public class CreateEventHandler: IRequestHandler<CreateEventRequest, CreateEventResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateEventHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<CreateEventResponse> Handle(CreateEventRequest request, CancellationToken cancellationToken)
    {
        request.PlacesLeft = request.MaxParticipants;
        var entity = _mapper.Map<Event>(request);
        var res = await _unitOfWork.EventRepository.CreateAsync(entity, cancellationToken);
        
        await _unitOfWork.Save();
        
        return _mapper.Map<CreateEventResponse>(res);
    }
}