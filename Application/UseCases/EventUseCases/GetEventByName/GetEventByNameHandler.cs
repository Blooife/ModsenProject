using Application.Exceptions;
using AutoMapper;
using Domain.Repositories.Interfaces;
using MediatR;

namespace Application.UseCases.EventUseCases.GetEventByName;

public class GetEventByNameHandler: IRequestHandler<GetEventByNameRequest, GetEventByNameResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetEventByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<GetEventByNameResponse> Handle(GetEventByNameRequest request, CancellationToken cancellationToken)
    {
        var ev =  await _unitOfWork.EventRepository.GetByNameAsync(request.name, cancellationToken);
        if (ev == null)
        {
            throw new NotFoundException("Event", request.name);
        }
        return _mapper.Map<GetEventByNameResponse>(ev);
    }
}