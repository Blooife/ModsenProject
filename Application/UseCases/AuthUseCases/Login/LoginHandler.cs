using Application.Exceptions;
using Application.Models.Dtos;
using Application.Servicies.Interfaces;
using AutoMapper;
using Domain.Repositories.Interfaces;
using MediatR;

namespace Application.UseCases.AuthUseCases.Login;

public class LoginHandler: IRequestHandler<LoginRequest, LoginResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private IJwtTokenGenerator _jwtTokenGenerator;

    public LoginHandler(IUnitOfWork unitOfWork, IMapper mapper, IJwtTokenGenerator jwtTokenGenerator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _jwtTokenGenerator = jwtTokenGenerator;
    }
    public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UserRepository.GetByEmailAsync(request.Email, cancellationToken);
            
        if (user == null)
        {
            throw new LoginException(ExceptionMessages.LoginFailed);
        }

        var isValid = await _unitOfWork.UserRepository.CheckPasswordAsync(user, request.Password);

        if (isValid == false)
        {
            throw new LoginException(ExceptionMessages.LoginFailed);
        }

        var roles = await _unitOfWork.UserRepository.GetRolesAsync(user);
        var token = _jwtTokenGenerator.GenerateToken(user, roles);
            
        var refreshToken = _jwtTokenGenerator.CreateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenEndDate = DateTime.Now.AddDays(7).ToUniversalTime();

        await _unitOfWork.UserRepository.UpdateAsync(user, cancellationToken);
            
        await _unitOfWork.Save();
            
            
        var userDto = _mapper.Map<UserDto>(user);


        return new LoginResponse()
        {
            User = userDto,
            Token = token,
        };
    }
}