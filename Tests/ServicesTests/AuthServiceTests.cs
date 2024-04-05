using Application.Exceptions;
using Application.Models.Dtos;
using Application.Services.Implementations;
using Application.Servicies.Interfaces;
using AutoMapper;
using Domain.Models.Entities;
using Domain.Repositories.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Moq;
using ValidationException = Application.Exceptions.ValidationException;

namespace Tests.ServicesTests;

public class AuthServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<AbstractValidator<RegistrationRequestDto>> _validatorMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
            _mapperMock = new Mock<IMapper>();
            _validatorMock = new Mock<AbstractValidator<RegistrationRequestDto>>();
            _authService = new AuthService(_unitOfWorkMock.Object, _jwtTokenGeneratorMock.Object, _mapperMock.Object, _validatorMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_ValidDto_RegistersUser()
        {
            // Arrange
            var dto = new RegistrationRequestDto();
            var user = new ApplicationUser();
            var validationResult = new ValidationResult();

            _validatorMock.Setup(validator =>
                    validator.ValidateAsync(It.IsAny<ValidationContext<RegistrationRequestDto>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);
    
            _unitOfWorkMock.Setup(uow => uow.UserRepository.RegisterAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _mapperMock.Setup(mapper => mapper.Map<ApplicationUser>(dto)).Returns(user);

            _unitOfWorkMock.Setup(uow => uow.UserRepository.GetByEmailAsync(dto.Email, CancellationToken.None)).ReturnsAsync(user);

            // Act
            var result = await _authService.RegisterAsync(dto, CancellationToken.None);

            // Assert
            Assert.Equal("User registered successfully", result.Message);
        }


        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var dto = new LoginRequestDto();
            var user = new ApplicationUser();
            var token = "test_token";

            _unitOfWorkMock.Setup(uow => uow.UserRepository.GetByEmailAsync(dto.Email, CancellationToken.None)).ReturnsAsync(user);
            _unitOfWorkMock.Setup(uow => uow.UserRepository.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(true);
            _unitOfWorkMock.Setup(uow => uow.UserRepository.GetRolesAsync(user)).ReturnsAsync(new List<string>());
            _jwtTokenGeneratorMock.Setup(generator => generator.GenerateToken(user, It.IsAny<IEnumerable<string>>())).Returns(token);

            // Act
            var result = await _authService.LoginAsync(dto, CancellationToken.None);

            // Assert
            Assert.Equal(token, result.Token);
        }

        [Fact]
        public async Task RefreshToken_ValidRefreshToken_ReturnsToken()
        {
            // Arrange
            var refreshToken = "test_refresh_token";
            var user = new ApplicationUser();
            var token = "test_token";

            _unitOfWorkMock.Setup(uow => uow.UserRepository.GetByRefreshTokenAsync(refreshToken, CancellationToken.None)).ReturnsAsync(user);
            _unitOfWorkMock.Setup(uow => uow.UserRepository.GetRolesAsync(user)).ReturnsAsync(new List<string>());
            _jwtTokenGeneratorMock.Setup(generator => generator.GenerateToken(user, It.IsAny<IEnumerable<string>>())).Returns(token);

            // Act
            var result = await _authService.RefreshToken(refreshToken, CancellationToken.None);

            // Assert
            Assert.Equal(token, result.Token);
        }

        [Fact]
        public async Task LoginAsync_UserNotFound_ThrowsLoginException()
        {
            // Arrange
            var loginRequestDto = new LoginRequestDto { Email = "nonexistent@example.com", Password = "password" };
            ApplicationUser user = null;

            _unitOfWorkMock.Setup(uow => uow.UserRepository.GetByEmailAsync(loginRequestDto.Email, CancellationToken.None)).ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<LoginException>(async () => await _authService.LoginAsync(loginRequestDto, CancellationToken.None));
        }
        
        [Fact]
        public async Task RegisterAsync_InvalidDto_ThrowsValidationException()
        {
            // Arrange
            var invalidDto = new RegistrationRequestDto();
            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Email", "Email is required") });

            _validatorMock.Setup(validator =>
                    validator.ValidateAsync(It.IsAny<ValidationContext<RegistrationRequestDto>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(async () => await _authService.RegisterAsync(invalidDto, CancellationToken.None));
        }

        [Fact]
        public async Task RegisterAsync_RegisterFailure_ThrowsRegisterException()
        {
            // Arrange
            var validDto = new RegistrationRequestDto();
            var user = new ApplicationUser();
            var registerResult = IdentityResult.Failed(new IdentityError { Description = "Failed to register user" });

            _validatorMock.Setup(validator =>
                    validator.ValidateAsync(It.IsAny<ValidationContext<RegistrationRequestDto>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _unitOfWorkMock.Setup(uow => uow.UserRepository.RegisterAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(registerResult);

            // Act & Assert
            await Assert.ThrowsAsync<RegisterException>(async () => await _authService.RegisterAsync(validDto, CancellationToken.None));
        }


}