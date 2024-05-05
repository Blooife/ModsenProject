using Application.Exceptions;
using AutoMapper;
using Domain.Repositories.Interfaces;
using MediatR;

namespace Application.UseCases.EventUseCases.DeleteEvent;

public class DeleteEventHandler: IRequestHandler<DeleteEventRequest, DeleteEventResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public DeleteEventHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<DeleteEventResponse> Handle(DeleteEventRequest request, CancellationToken cancellationToken)
    {
        var entity = await _unitOfWork.EventRepository.GetByIdAsync(request.id, cancellationToken);
        if (entity == null)
        {
            throw new NotFoundException("Event", request.id);
        }
        await _unitOfWork.EventRepository.DeleteAsync(entity, cancellationToken);
        
        await _unitOfWork.Save();
        return _mapper.Map<DeleteEventResponse>(entity);
    }
}