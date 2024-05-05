using Application.Exceptions;
using Application.Models.Dtos;
using Application.Servicies.Interfaces;
using AutoMapper;
using Domain.Models.Entities;
using Domain.Repositories.Interfaces;
using FluentValidation;
using MediatR;

namespace Application.UseCases.AuthUseCases.RefreshToken;

public class RefreshTokenHandler: IRequestHandler<RefreshTokenRequest, RefreshTokenResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private IJwtTokenGenerator _jwtTokenGenerator;

    public RefreshTokenHandler(IUnitOfWork unitOfWork, IMapper mapper, IJwtTokenGenerator jwtTokenGenerator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _jwtTokenGenerator = jwtTokenGenerator;
    }
    public async Task<RefreshTokenResponse> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UserRepository.GetByRefreshTokenAsync(request.refreshToken, cancellationToken);
            
        if (user == null)
        {
            throw new LoginException(ExceptionMessages.LoginFailed);
        }
            
        if(user.RefreshTokenEndDate < DateTime.Now)
        {
            throw new LoginException("Refresh token expired");
        }

        var roles = await _unitOfWork.UserRepository.GetRolesAsync(user);
        var token = _jwtTokenGenerator.GenerateToken(user, roles);
            
        var refreshToken = _jwtTokenGenerator.CreateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenEndDate = DateTime.Now.AddDays(7).ToUniversalTime();

        await _unitOfWork.UserRepository.UpdateAsync(user, cancellationToken);
            
        await _unitOfWork.Save();
            
        var userDto = _mapper.Map<UserDto>(user);
        
        return new RefreshTokenResponse()
        {
            User = userDto,
            Token = token,
        };
    }
}