using Application.Exceptions;
using Application.Models.Dtos;
using Application.Servicies.Interfaces;
using Application.UseCases.AuthUseCases.AssignRole;
using Application.UseCases.AuthUseCases.CreateRole;
using Application.UseCases.AuthUseCases.Login;
using Application.UseCases.AuthUseCases.Register;
using AutoMapper;
using Domain.Models.Entities;
using Domain.Repositories.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Tests.UseCasesTests;

public class AuthUseCasesTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    
    [Fact]
        public async Task Handle_WithValidRequest_ReturnsSuccessResponse()
        {
            // Arrange
            var request = new AssignRoleRequest
            {
                Email = "test@example.com",
                Role = "Admin"
            };
            var user = new ApplicationUser() { Email = "test@example.com", /* Include other properties */ };

            _unitOfWorkMock.Setup(uow => uow.UserRepository.GetByNameAsync(request.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _unitOfWorkMock.Setup(uow => uow.UserRepository.RoleExistsAsync(request.Role.ToUpper()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(uow => uow.UserRepository.AddToRoleAsync(user, request.Role.ToUpper()))
                .Returns(Task.CompletedTask);

            var handler = new AssignRoleHandler(_unitOfWorkMock.Object);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Message.Should().Be("Role assigned successfully");
            _unitOfWorkMock.Verify(uow => uow.UserRepository.GetByNameAsync(request.Email, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.UserRepository.RoleExistsAsync(request.Role.ToUpper()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.UserRepository.AddToRoleAsync(user, request.Role.ToUpper()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithInvalidUser_ThrowsAssignRoleException()
        {
            // Arrange
            var request = new AssignRoleRequest
            {
                Email = "test@example.com",
                Role = "Admin"
            };

            _unitOfWorkMock.Setup(uow => uow.UserRepository.GetByNameAsync(request.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ApplicationUser)null);

            var handler = new AssignRoleHandler(_unitOfWorkMock.Object);

            // Act
            // Assert
            await Assert.ThrowsAsync<AssignRoleException>(async () => await handler.Handle(request, CancellationToken.None));
            _unitOfWorkMock.Verify(uow => uow.UserRepository.GetByNameAsync(request.Email, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.UserRepository.RoleExistsAsync(It.IsAny<string>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.UserRepository.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WithInvalidRole_ThrowsAssignRoleException()
        {
            // Arrange
            var request = new AssignRoleRequest
            {
                Email = "test@example.com",
                Role = "InvalidRole"
            };
            var user = new ApplicationUser(){ Email = "test@example.com",  };

            _unitOfWorkMock.Setup(uow => uow.UserRepository.GetByNameAsync(request.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _unitOfWorkMock.Setup(uow => uow.UserRepository.RoleExistsAsync(request.Role.ToUpper()))
                .ReturnsAsync(false);

            var handler = new AssignRoleHandler(_unitOfWorkMock.Object);

            // Act
            // Assert
            await Assert.ThrowsAsync<AssignRoleException>(async () => await handler.Handle(request, CancellationToken.None));
            _unitOfWorkMock.Verify(uow => uow.UserRepository.GetByNameAsync(request.Email, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.UserRepository.RoleExistsAsync(request.Role.ToUpper()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.UserRepository.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        }
        
        [Fact]
        public async Task CreateRoleHandle_WithValidRequest_ReturnsSuccessResponse()
        {
            // Arrange
            var request = new CreateRoleRequest("Admin");
            var identityResult = IdentityResult.Success;

            _unitOfWorkMock.Setup(uow => uow.UserRepository.CreateRoleAsync(request.roleName))
                .ReturnsAsync(identityResult);

            var handler = new CreateRoleHandler(_unitOfWorkMock.Object);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Message.Should().Be("Role created successfully");
            _unitOfWorkMock.Verify(uow => uow.UserRepository.CreateRoleAsync(request.roleName), Times.Once);
        }

        [Fact]
        public async Task CreateRoleHandle_WithInvalidRequest_ThrowsCreateRoleException()
        {
            // Arrange
            var request = new CreateRoleRequest("Invalid role");
            var identityResult = IdentityResult.Failed(new IdentityError { Description = "Error creating role" });

            _unitOfWorkMock.Setup(uow => uow.UserRepository.CreateRoleAsync(request.roleName))
                .ReturnsAsync(identityResult);

            var handler = new CreateRoleHandler(_unitOfWorkMock.Object);

            // Act
            // Assert
            await Assert.ThrowsAsync<CreateRoleException>(async () => await handler.Handle(request, CancellationToken.None));
            _unitOfWorkMock.Verify(uow => uow.UserRepository.CreateRoleAsync(request.roleName), Times.Once);
        }
        
        [Fact]
        public async Task LoginHandle_ValidCredentials_ReturnsLoginResponse()
        {
            // Arrange
            var user = new ApplicationUser(){ Email = "test@example.com" };
            var userDto = new UserDto { Email = "test@example.com" };
            var roles = new List<string> { "Admin" };
            var token = "test_token";

            
            var mapperMock = new Mock<IMapper>();
            var jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();

            var request = new LoginRequest { Email = "test@example.com", Password = "password" };

            _unitOfWorkMock.Setup(uow => uow.UserRepository.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _unitOfWorkMock.Setup(uow => uow.UserRepository.CheckPasswordAsync(user, request.Password))
                .ReturnsAsync(true);

            _unitOfWorkMock.Setup(uow => uow.UserRepository.GetRolesAsync(user))
                .ReturnsAsync(roles);

            jwtTokenGeneratorMock.Setup(jwt => jwt.GenerateToken(user, roles))
                .Returns(token);

            mapperMock.Setup(map => map.Map<UserDto>(user))
                .Returns(userDto);

            var handler = new LoginHandler(_unitOfWorkMock.Object, mapperMock.Object, jwtTokenGeneratorMock.Object);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userDto, result.User);
            Assert.Equal(token, result.Token);
            _unitOfWorkMock.Verify(uow => uow.Save(), Times.Once);
        }

        [Fact]
        public async Task LoginHandle_InvalidCredentials_ThrowsLoginException()
        {
            // Arrange
            var mapperMock = new Mock<IMapper>();
            var jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();

            var request = new LoginRequest { Email = "test@example.com", Password = "wrong_password" };

            _unitOfWorkMock.Setup(uow => uow.UserRepository.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ApplicationUser)null);

            var handler = new LoginHandler(_unitOfWorkMock.Object, mapperMock.Object, jwtTokenGeneratorMock.Object);

            // Act
            // Assert
            await Assert.ThrowsAsync<LoginException>(async () => await handler.Handle(request, CancellationToken.None));
            _unitOfWorkMock.Verify(uow => uow.Save(), Times.Never);
        }
        
        [Fact]
        public async Task RegisterHandle_ValidRegistration_ReturnsRegisterResponse()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "test@example.com",
                Password = "P@ssw0rd",
            };

            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Email
            };

            
            var mapperMock = new Mock<IMapper>();

            _unitOfWorkMock.Setup(uow => uow.UserRepository.RegisterAsync(user, request.Password))
                .ReturnsAsync(IdentityResult.Success);

            _unitOfWorkMock.Setup(uow => uow.UserRepository.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            mapperMock.Setup(map => map.Map<ApplicationUser>(request))
                .Returns(user);

            var handler = new RegisterHandler(_unitOfWorkMock.Object, mapperMock.Object);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("User registered successfully", result.Message);
        }

        [Fact]
        public async Task RegisterHandle_InvalidRegistration_ThrowsRegisterException()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "test@example.com",
                Password = "P@ssw0rd",
            };

            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Email
            };

            var errors = new List<IdentityError> { new IdentityError { Description = "Email already exists" } };

            
            var mapperMock = new Mock<IMapper>();

            _unitOfWorkMock.Setup(uow => uow.UserRepository.RegisterAsync(user, request.Password))
                .ReturnsAsync(IdentityResult.Failed(errors.ToArray()));

            mapperMock.Setup(map => map.Map<ApplicationUser>(request))
                .Returns(user);

            var handler = new RegisterHandler(_unitOfWorkMock.Object, mapperMock.Object);

            // Act
            // Assert
            await Assert.ThrowsAsync<RegisterException>(async () => await handler.Handle(request, CancellationToken.None));
            _unitOfWorkMock.Verify(uow => uow.Save(), Times.Never);
        }
}