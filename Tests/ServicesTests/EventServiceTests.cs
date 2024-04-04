using Application.Exceptions;
using Application.Models.Dtos;
using Application.Servicies.Implementations;
using Application.Servicies.Interfaces;
using AutoMapper;
using Domain.Models.Entities;
using Domain.Repositories.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using JetBrains.Annotations;
using FluentAssertions;

using Moq;
using ValidationException = Application.Exceptions.ValidationException;

namespace Tests.ServicesTests;

[TestSubject(typeof(EventService))]
public class EventServiceTests
{

    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<AbstractValidator<EventRequestDto>> _validatorMock = new();
    private readonly IEventService _eventService;

    public EventServiceTests()
    {
        _eventService = new EventService(_unitOfWorkMock.Object, _mapperMock.Object, _validatorMock.Object);
    }
    
    [Fact]
    public async Task CreateAsync_InvalidArgument_ThrowsValidationException()
    {
        EventRequestDto editorRequestDto = new();
        Event editor = new();
        ValidationResult validationResult =
            new ValidationResult(new List<ValidationFailure>([new ValidationFailure()]));
        _mapperMock.Setup(mapper => mapper.Map<Event>(It.IsAny<EventRequestDto>())).Returns(editor);
        _validatorMock.Setup(validator =>
                validator.ValidateAsync(It.IsAny<ValidationContext<EventRequestDto>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        Func<Task> actual = async () => await _eventService.CreateAsync(editorRequestDto);
        
        await actual.Should().ThrowExactlyAsync<ValidationException>();
        _validatorMock.Verify(
            validator => validator.ValidateAsync(It.IsAny<ValidationContext<EventRequestDto>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task CreateAsync_ValidArgument_ReturnsAddedEvent()
    {
        // Arrange
        EventRequestDto eventRequestDto = new EventRequestDto();
        Event createdEvent = new Event();

        _mapperMock.Setup(mapper => mapper.Map<Event>(It.IsAny<EventRequestDto>()))
            .Returns(new Event());
        
        _validatorMock.Setup(validator =>
                validator.ValidateAsync(It.IsAny<ValidationContext<EventRequestDto>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _unitOfWorkMock.Setup(uow => uow.EventRepository.CreateAsync(It.IsAny<Event>()))
            .ReturnsAsync(createdEvent);
        _unitOfWorkMock.Setup(uow => uow.Save()).Returns(Task.FromResult(1));
        
        // Act
        var result = await _eventService.CreateAsync(eventRequestDto);

        // Assert
        result.Should().NotBeNull();
        _mapperMock.Verify(mapper => mapper.Map<Event>(It.IsAny<EventRequestDto>()), Times.Once);
        _validatorMock.Verify(
            validator => validator.ValidateAsync(It.IsAny<ValidationContext<EventRequestDto>>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(uow => uow.EventRepository.CreateAsync(It.IsAny<Event>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Save(), Times.Once);
    }
    
    [Fact]
    public async Task GetAllAsync_ReturnsAllEvents()
    {
        // Arrange
        IEnumerable<Event> events = new List<Event>() { new Event(), new Event() };
        _unitOfWorkMock.Setup(uow => uow.EventRepository.GetAllAsync()).ReturnsAsync(events);

        // Act
        var result = await _eventService.GetAllAsync();

        // Assert
        Assert.Equal(events, result);
    }
    
    [Fact]
    public async Task GetByIdAsync_ValidId_ReturnsEvent()
    {
        // Arrange
        string eventId = "1";
        Event expectedEvent = new Event();
        _unitOfWorkMock.Setup(uow => uow.EventRepository.GetByIdAsync(eventId)).ReturnsAsync(expectedEvent);

        // Act
        var result = await _eventService.GetByIdAsync(eventId);

        // Assert
        Assert.Equal(expectedEvent, result);
    }

    [Fact]
    public async Task CreateAsync_ValidDto_CreatesEvent()
    {
        // Arrange
        EventRequestDto dto = new EventRequestDto();
        Event createdEvent = new Event();
        _validatorMock.Setup(validator =>
                validator.ValidateAsync(It.IsAny<ValidationContext<EventRequestDto>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(uow => uow.EventRepository.CreateAsync(It.IsAny<Event>())).ReturnsAsync(createdEvent);
        _unitOfWorkMock.Setup(uow => uow.Save()).Returns(Task.FromResult(1));

        // Act
        var result = await _eventService.CreateAsync(dto);

        // Assert
        Assert.Equal(createdEvent, result);
    }
    
    [Fact]
    public async Task DeleteAsync_ValidId_DeletesEvent()
    {
        // Arrange
        string eventId = "1";
        Event deletedEvent = new Event();
        _unitOfWorkMock.Setup(uow => uow.EventRepository.GetByIdAsync(eventId)).ReturnsAsync(deletedEvent);
        _unitOfWorkMock.Setup(uow => uow.EventRepository.DeleteAsync(deletedEvent)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(uow => uow.Save()).Returns(Task.FromResult(1));

        // Act
        var result = await _eventService.DeleteAsync(eventId);

        // Assert
        Assert.Equal(deletedEvent, result);
    }
    
    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsEvent()
    {
        // Arrange
        string eventId = "existingId";
        Event existingEvent = new Event();
        _unitOfWorkMock.Setup(uow => uow.EventRepository.GetByIdAsync(eventId)).ReturnsAsync(existingEvent);

        // Act
        var result = await _eventService.GetByIdAsync(eventId);

        // Assert
        Assert.Equal(existingEvent, result);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ThrowsNotFoundException()
    {
        // Arrange
        string eventId = "nonExistingId";
        _unitOfWorkMock.Setup(uow => uow.EventRepository.GetByIdAsync(eventId)).ReturnsAsync((Event)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => await _eventService.GetByIdAsync(eventId));
    }

    [Fact]
    public async Task DeleteAsync_ExistingId_DeletesEvent()
    {
        // Arrange
        string eventId = "existingId";
        Event existingEvent = new Event();
        _unitOfWorkMock.Setup(uow => uow.EventRepository.GetByIdAsync(eventId)).ReturnsAsync(existingEvent);

        // Act
        var result = await _eventService.DeleteAsync(eventId);

        // Assert
        Assert.Equal(existingEvent, result);
        _unitOfWorkMock.Verify(uow => uow.EventRepository.DeleteAsync(existingEvent), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Save(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingId_ThrowsNotFoundException()
    {
        // Arrange
        string eventId = "nonExistingId";
        _unitOfWorkMock.Setup(uow => uow.EventRepository.GetByIdAsync(eventId)).ReturnsAsync((Event)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => await _eventService.DeleteAsync(eventId));
    }

    [Fact]
    public async Task GetByNameAsync_ExistingName_ReturnsEvent()
    {
        // Arrange
        string eventName = "existingName";
        Event existingEvent = new Event();
        _unitOfWorkMock.Setup(uow => uow.EventRepository.GetByNameAsync(eventName)).ReturnsAsync(existingEvent);

        // Act
        var result = await _eventService.GetByNameAsync(eventName);

        // Assert
        Assert.Equal(existingEvent, result);
    }

    [Fact]
    public async Task GetByNameAsync_NonExistingName_ThrowsNotFoundException()
    {
        // Arrange
        string eventName = "nonExistingName";
        _unitOfWorkMock.Setup(uow => uow.EventRepository.GetByNameAsync(eventName)).ReturnsAsync((Event)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => await _eventService.GetByNameAsync(eventName));
    }

    

    [Fact]
    public async Task GetByCategoryAsync_ExistingCategory_ReturnsEvents()
    {
        // Arrange
        string category = "existingCategory";
        IEnumerable<Event> events = new List<Event> { new Event(), new Event() };
        _unitOfWorkMock.Setup(uow => uow.EventRepository.GetByCategoryAsync(category)).ReturnsAsync(events);

        // Act
        var result = await _eventService.GetByCategoryAsync(category);

        // Assert
        Assert.Equal(events, result);
    }

    [Fact]
    public async Task GetByCategoryAsync_NonExistingCategory_ReturnsEmptyList()
    {
        // Arrange
        string category = "nonExistingCategory";
        _unitOfWorkMock.Setup(uow => uow.EventRepository.GetByCategoryAsync(category)).ReturnsAsync(new List<Event>());

        // Act
        var result = await _eventService.GetByCategoryAsync(category);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByPlaceAsync_ExistingPlace_ReturnsEvents()
    {
        // Arrange
        string place = "existingPlace";
        IEnumerable<Event> events = new List<Event> { new Event(), new Event() };
        _unitOfWorkMock.Setup(uow => uow.EventRepository.GetByPlaceAsync(place)).ReturnsAsync(events);

        // Act
        var result = await _eventService.GetByPlaceAsync(place);

        // Assert
        Assert.Equal(events, result);
    }

    [Fact]
    public async Task GetByPlaceAsync_NonExistingPlace_ReturnsEmptyList()
    {
        // Arrange
        string place = "nonExistingPlace";
        _unitOfWorkMock.Setup(uow => uow.EventRepository.GetByPlaceAsync(place)).ReturnsAsync(new List<Event>());

        // Act
        var result = await _eventService.GetByPlaceAsync(place);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByDateAsync_ExistingDate_ReturnsEvents()
    {
        // Arrange
        DateTime date = DateTime.UtcNow;
        IEnumerable<Event> events = new List<Event> { new Event(), new Event() };
        _unitOfWorkMock.Setup(uow => uow.EventRepository.GetByDateAsync(date)).ReturnsAsync(events);

        // Act
        var result = await _eventService.GetByDateAsync(date);

        // Assert
        Assert.Equal(events, result);
    }

    [Fact]
    public async Task GetByDateAsync_NonExistingDate_ReturnsEmptyList()
    {
        // Arrange
        DateTime date = DateTime.UtcNow;
        _unitOfWorkMock.Setup(uow => uow.EventRepository.GetByDateAsync(date)).ReturnsAsync(new List<Event>());

        // Act
        var result = await _eventService.GetByDateAsync(date);

        // Assert
        Assert.Empty(result);
    }
}
