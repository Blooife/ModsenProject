using Application.Exceptions;
using AutoMapper;
using Domain.Models.Entities;
using Domain.Repositories.Interfaces;
using MediatR;

namespace Application.UseCases.EventUseCases.UpdateEvent;

public class UpdateEventHandler: IRequestHandler<UpdateEventRequest, UpdateEventResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateEventHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<UpdateEventResponse> Handle(UpdateEventRequest request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<Event>(request);
        if (! await _unitOfWork.EventRepository.Exists(entity.Id, cancellationToken))
        {
            throw new NotFoundException("Event", entity.Id);
        }

        var updateResult = await _unitOfWork.EventRepository.UpdatePlacesLeftAsync(entity, 0, cancellationToken);

        if (!updateResult)
        {
            throw new BadRequestException("you cant update participants count");
        }
        
        await _unitOfWork.Save();
        
        return _mapper.Map<UpdateEventResponse>(entity);
    }
}