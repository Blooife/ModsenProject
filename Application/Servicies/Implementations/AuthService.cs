using System.ComponentModel;
using System.Data;
using Application.Exceptions;
using Application.Models.Dtos;
using Application.Servicies.Interfaces;
using AutoMapper;
using Domain.Models.Entities;
using Domain.Repositories.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ValidationException = Application.Exceptions.ValidationException;

namespace Application.Servicies.Implementations;

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

        public async Task<ResponseDto> RegisterAsync(RegistrationRequestDto registrationRequestDto)
        {
            var validationResult = await _validator.ValidateAsync(registrationRequestDto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult);
            }
            var user = _mapper.Map<ApplicationUser>(registrationRequestDto);
            try
            {
                var result = await _unitOfWork.UserRepository.RegisterAsync(user, registrationRequestDto.Password);
                if (result.Succeeded)
                {
                    var userToReturn = await _unitOfWork.UserRepository.GetByEmailAsync(registrationRequestDto.Email);

                    var userDto = _mapper.Map<UserDto>(userToReturn);

                    return new ResponseDto { Message = "User registered successfully"};
                }
                else
                {
                    throw new RegisterException(result.Errors.FirstOrDefault().Description);
                }
            }
            catch (DbUpdateException ex)
            {
                throw new DataException("db exception");
            }
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto)
        {
            var user = await _unitOfWork.UserRepository.GetByEmailAsync(loginRequestDto.Email);
            
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

            await _unitOfWork.UserRepository.UpdateAsync(user);
            try
            {
                await _unitOfWork.Save();
            }
            catch (DbUpdateException ex)
            {
                throw new DbException("");
            }
            
            
            var userDto = _mapper.Map<UserDto>(user);

            var loginResponseDto = new LoginResponseDto
            {
                User = userDto,
                Token = token,
                Role = roles,
            };

            return loginResponseDto;
        }
        
        public async Task<LoginResponseDto> RefreshToken(string refrToken)
        {
            var user = await _unitOfWork.UserRepository.GetByRefreshTokenAsync(refrToken);
            
            if (user == null)
            {
                throw new LoginException(ExceptionMessages.LoginFailed);
            }
            
            if(user.RefreshTokenEndDate < DateTime.Now)
            {
                throw new LoginException("Refresh token expired");
            }

            //var isValid = await _unitOfWork.UserRepository.CheckPasswordAsync(user, loginRequestDto.Password);

            var roles = await _unitOfWork.UserRepository.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user, roles);
            
            
            
            var refreshToken = _jwtTokenGenerator.CreateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenEndDate = DateTime.Now.AddDays(7).ToUniversalTime();

            await _unitOfWork.UserRepository.UpdateAsync(user);
            try
            {
                await _unitOfWork.Save();
            }
            catch (DbUpdateException ex)
            {
                throw new DbException("");
            }
            
            
            
            var userDto = _mapper.Map<UserDto>(user);

            var loginResponseDto = new LoginResponseDto
            {
                User = userDto,
                Token = token,
                Role = roles,
            };

            return loginResponseDto;
        }

        public async Task<ResponseDto> AssignRoleAsync(string name, string roleName)
        {
            var user = await _unitOfWork.UserRepository.GetByNameAsync(name);
            if (user != null)
            {
                var isRoleExist = await _unitOfWork.UserRepository.RoleExistsAsync(roleName);
                if (!isRoleExist)
                {
                    await _unitOfWork.UserRepository.CreateRoleAsync(roleName);
                }

                await _unitOfWork.UserRepository.AddToRoleAsync(user, roleName);

                return new ResponseDto { Message = "Role assigned successfully" };
            }
            else
            {
                throw new AssignRoleException(ExceptionMessages.ErrorAssigningRole);
            }
        }

        public async Task<ResponseDto> RegisterUserOnEvent(string userId, string eventId)
        {
            var ev = await _unitOfWork.EventRepository.GetByIdAsync(eventId);
            if (ev == null)
            {
                throw new NotFoundException("Event", eventId);
            }

            if (ev.PlacesLeft < 1)
            {
                throw new ArgumentException("No places left");
            }
            if (!await _unitOfWork.UserRepository.Exists(userId))
            {
                throw new NotFoundException("User", userId);
            }
            await _unitOfWork.EventsUsersRepository.RegisterUserOnEvent(userId, eventId);
            await _unitOfWork.EventRepository.UpdatePlacesLeftAsync(ev, 1);
            try
            {
                await _unitOfWork.Save();
            }
            catch(DbUpdateException ex)
            {
                throw new DbException("db exception");
            }
            return new ResponseDto { Message = "User successfully registered on event" };
        }
        
        public async Task<ResponseDto> UnregisterUserOnEvent(string userId, string eventId)
        {
            var ev = await _unitOfWork.EventRepository.GetByIdAsync(eventId);
            if (ev == null)
            {
                throw new NotFoundException("Event", eventId);
            }
            if (!await _unitOfWork.UserRepository.Exists(userId))
            {
                throw new NotFoundException("User", userId);
            }
            await _unitOfWork.EventsUsersRepository.UnregisterUserOnEvent(userId, eventId);
            await _unitOfWork.EventRepository.UpdatePlacesLeftAsync(ev, -1);
            try
            {
                await _unitOfWork.Save();
            }
            catch(DbUpdateException ex)
            {
                throw new DbException("db exception");
            }
            return new ResponseDto { Message = "User successfully unregistered on event" };
        }

        public async Task<IEnumerable<Event>> GetAllUserEvents(string userId)
        {
            return await _unitOfWork.UserRepository.GetAllUserEvents(userId);
        }
}