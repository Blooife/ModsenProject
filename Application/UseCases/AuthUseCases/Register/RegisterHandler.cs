using Application.Exceptions;
using AutoMapper;
using Domain.Models.Entities;
using Domain.Repositories.Interfaces;
using MediatR;

namespace Application.UseCases.AuthUseCases.Register;

public class RegisterHandler: IRequestHandler<RegisterRequest, RegisterResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RegisterHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<RegisterResponse> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        var user = _mapper.Map<ApplicationUser>(request);
            
        var result = await _unitOfWork.UserRepository.RegisterAsync(user, request.Password);
        if (result.Succeeded)
        {
            await _unitOfWork.UserRepository.GetByEmailAsync(request.Email, cancellationToken);

            return new RegisterResponse()
            {
                Message = "User registered successfully",
            };
        }
        else
        {
            throw new RegisterException(result.Errors.FirstOrDefault().Description);
        }
    }
}