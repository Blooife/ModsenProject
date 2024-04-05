using Application.Exceptions;
using Application.Models.Dtos;
using Application.Servicies.Interfaces;
using AutoMapper;
using Domain.Models.Entities;
using Domain.Repositories.Interfaces;
using FluentValidation;
using ValidationException = Application.Exceptions.ValidationException;

namespace Application.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IMapper _mapper;
        private readonly AbstractValidator<RegistrationRequestDto> _validator;

        public AuthService(IUnitOfWork unitOfWork, IJwtTokenGenerator jwtTokenGenerator, IMapper mapper, AbstractValidator<RegistrationRequestDto> validator)
        {
            _unitOfWork = unitOfWork;
            _jwtTokenGenerator = jwtTokenGenerator;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<ResponseDto> RegisterAsync(RegistrationRequestDto registrationRequestDto, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(registrationRequestDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult);
            }
            var user = _mapper.Map<ApplicationUser>(registrationRequestDto);
            
                var result = await _unitOfWork.UserRepository.RegisterAsync(user, registrationRequestDto.Password);
                if (result.Succeeded)
                {
                    await _unitOfWork.UserRepository.GetByEmailAsync(registrationRequestDto.Email, cancellationToken);

                    return new ResponseDto { Message = "User registered successfully"};
                }
                else
                {
                    throw new RegisterException(result.Errors.FirstOrDefault().Description);
                }
            
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.GetByEmailAsync(loginRequestDto.Email, cancellationToken);
            
            if (user == null)
            {
                throw new LoginException(ExceptionMessages.LoginFailed);
            }

            var isValid = await _unitOfWork.UserRepository.CheckPasswordAsync(user, loginRequestDto.Password);

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

            var loginResponseDto = new LoginResponseDto
            {
                User = userDto,
                Token = token,
            };

            return loginResponseDto;
        }
        
        public async Task<LoginResponseDto> RefreshToken(string refrToken, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.GetByRefreshTokenAsync(refrToken, cancellationToken);
            
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

            var loginResponseDto = new LoginResponseDto
            {
                User = userDto,
                Token = token,
            };

            return loginResponseDto;
        }

        public async Task<ResponseDto> CreateRoleAsync(string roleName)
        {
            var result = await _unitOfWork.UserRepository.CreateRoleAsync(roleName);
            if (!result.Succeeded)
            {
                throw new CreateRoleException(ExceptionMessages.CreateRoleFailed);
            }
            return new ResponseDto() { Message = "Role created successfully" };
        }

        public async Task<ResponseDto> AssignRoleAsync(string name, string roleName, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.GetByNameAsync(name, cancellationToken);
            if (user != null)
            {
                var isRoleExist = await _unitOfWork.UserRepository.RoleExistsAsync(roleName);
                if (!isRoleExist)
                {
                    throw new AssignRoleException(ExceptionMessages.RoleNotExists);
                }
                await _unitOfWork.UserRepository.AddToRoleAsync(user, roleName);

                return new ResponseDto { Message = "Role assigned successfully" };
            }
            throw new AssignRoleException(ExceptionMessages.ErrorAssigningRole);
        }
}