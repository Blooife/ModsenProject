using Application.Exceptions;
using AutoMapper;
using Domain.Repositories.Interfaces;
using MediatR;

namespace Application.UseCases.EventUseCases.GetEventById;

public class GetEventByIdHandler: IRequestHandler<GetEventByIdRequest, GetEventByIdResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetEventByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<GetEventByIdResponse> Handle(GetEventByIdRequest request, CancellationToken cancellationToken)
    {
        var ev = await _unitOfWork.EventRepository.GetByIdAsync(request.id, cancellationToken);
        if (ev == null)
        {
            throw new NotFoundException("Event", request.id);
        }
        return _mapper.Map<GetEventByIdResponse>(ev);
    }
}