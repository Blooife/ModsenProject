using Application.Exceptions;
using Application.UseCases.EventUseCases.CreateEvent;
using Application.UseCases.EventUseCases.DeleteEvent;
using Application.UseCases.EventUseCases.GetAllEvents;
using Application.UseCases.EventUseCases.GetAllUserEvents;
using Application.UseCases.EventUseCases.GetEventByCategory;
using Application.UseCases.EventUseCases.GetEventByDate;
using Application.UseCases.EventUseCases.GetEventById;
using Application.UseCases.EventUseCases.GetFilteredEvents;
using Application.UseCases.EventUseCases.RegisterUserOnEvent;
using Application.UseCases.EventUseCases.UnRegisterUserOnEvent;
using Application.UseCases.EventUseCases.UpdateEvent;
using AutoMapper;
using Domain.Models.Entities;
using Domain.Repositories.Interfaces;
using FluentAssertions;
using Moq;

namespace Tests.UseCasesTests;

public class EventUseCasesTests
{
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<IMapper> _mapperMock = new();

        [Fact]
        public async Task CreateEvent_ValidRequest_CreatesEvent()
        {
            // Arrange
            var request = new CreateEventRequest {  };
            var mappedEntity = new Event(); 
            var expectedResponse = new CreateEventResponse(); 
            _mapperMock.Setup(m => m.Map<Event>(request)).Returns(mappedEntity);
            _unitOfWorkMock.Setup(u => u.EventRepository.CreateAsync(It.IsAny<Event>(), It.IsAny<CancellationToken>())).ReturnsAsync(mappedEntity);
            _mapperMock.Setup(m => m.Map<CreateEventResponse>(mappedEntity)).Returns(expectedResponse);
            var createEventHandler = new CreateEventHandler(_unitOfWorkMock.Object, _mapperMock.Object);
            // Act
            var result = await createEventHandler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResponse);
        }
        
        [Fact]
        public async Task Handle_ValidId_ReturnsEvent()
        {
            // Arrange
            string eventId = "validId";
            var expectedEvent = new Event { Id = eventId }; 
            var request = new GetEventByIdRequest (eventId );
            
            
            
            _unitOfWorkMock.Setup(u => u.EventRepository.GetByIdAsync(eventId, CancellationToken.None)).ReturnsAsync(expectedEvent);
            _mapperMock.Setup(m => m.Map<GetEventByIdResponse>(expectedEvent)).Returns(new GetEventByIdResponse { Id = eventId });

            var handler = new GetEventByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(eventId);
        }

        [Fact]
        public async Task Handle_InvalidId_ThrowsNotFoundException()
        {
            // Arrange
            string invalidId = "invalidId";
            var request = new GetEventByIdRequest (invalidId );
            
            
            
            _unitOfWorkMock.Setup(u => u.EventRepository.GetByIdAsync(invalidId, CancellationToken.None)).ReturnsAsync((Event)null);

            var handler = new GetEventByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(request, CancellationToken.None));
        }
        
        [Fact]
        public async Task Handle_ExistingEvent_DeletesEvent()
        {
            // Arrange
            string eventId = "existingId";
            var request = new DeleteEventRequest (eventId );
            var existingEvent = new Event { Id = eventId };

            
            
            _unitOfWorkMock.Setup(u => u.EventRepository.GetByIdAsync(eventId, CancellationToken.None)).ReturnsAsync(existingEvent);
            _unitOfWorkMock.Setup(u => u.EventRepository.DeleteAsync(existingEvent, CancellationToken.None)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.Save()).Returns(Task.FromResult(1));
            _mapperMock.Setup(m => m.Map<DeleteEventResponse>(existingEvent)).Returns(new DeleteEventResponse { Id = eventId });

            var handler = new DeleteEventHandler(_unitOfWorkMock.Object, _mapperMock.Object);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(eventId);
            _unitOfWorkMock.Verify(u => u.EventRepository.DeleteAsync(existingEvent, CancellationToken.None), Times.Once);
            _unitOfWorkMock.Verify(u => u.Save(), Times.Once);
        }

        [Fact]
        public async Task Handle_NonExistingEvent_ThrowsNotFoundException()
        {
            // Arrange
            string nonExistingId = "nonExistingId";
            var request = new DeleteEventRequest(nonExistingId);

            
            
            _unitOfWorkMock.Setup(u => u.EventRepository.GetByIdAsync(nonExistingId, CancellationToken.None)).ReturnsAsync((Event)null);

            var handler = new DeleteEventHandler(_unitOfWorkMock.Object, _mapperMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(request, CancellationToken.None));
            _unitOfWorkMock.Verify(u => u.EventRepository.DeleteAsync(It.IsAny<Event>(), CancellationToken.None), Times.Never);
            _unitOfWorkMock.Verify(u => u.Save(), Times.Never);
        }
        
        [Fact]
        public async Task Handle_ReturnsAllEvents()
        {
            // Arrange
            var request = new GetAllEventsRequest();
            var allEvents = new List<Event>
            {
                new Event { Id = "1", Name = "Event 1" },
                new Event { Id = "2", Name = "Event 2" },
                new Event { Id = "3", Name = "Event 3" }
            };

            
            _unitOfWorkMock.Setup(u => u.EventRepository.GetAllAsync(CancellationToken.None)).ReturnsAsync(allEvents);

            var handler = new GetAllEventsHandler(_unitOfWorkMock.Object);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(allEvents.Count, result.Events.Count());
            foreach (var evt in allEvents)
            {
                Assert.Contains(evt, result.Events);
            }
        }
        
        [Fact]
        public async Task Handle_ReturnsUserEvents()
        {
            // Arrange
            var userId = "user123";
            var request = new GetAllUserEventsRequest (userId );
            var userEvents = new List<Event>
            {
                new Event { Id = "1", Name = "User Event 1" },
                new Event { Id = "2", Name = "User Event 2" },
                new Event { Id = "3", Name = "User Event 3" }
            };

            
            _unitOfWorkMock.Setup(u => u.EventsUsersRepository.GetAllUserEvents(userId, CancellationToken.None)).ReturnsAsync(userEvents);

            var handler = new GetAllUserEventsHandler(_unitOfWorkMock.Object);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userEvents.Count, result.events.Count());
            foreach (var evt in userEvents)
            {
                Assert.Contains(evt, result.events);
            }
        }
        
        [Fact]
        public async Task Handle_ReturnsEventsByCategory()
        {
            // Arrange
            var category = "Concerts";
            var request = new GetEventByCategoryRequest (category );
            var eventsByCategory = new List<Event>
            {
                new Event { Id = "1", Name = "Concert 1", Category = "Concerts" },
                new Event { Id = "2", Name = "Concert 2", Category = "Concerts" },
                new Event { Id = "3", Name = "Festival", Category = "Concerts" }
            };

            
            _unitOfWorkMock.Setup(u => u.EventRepository.GetByCategoryAsync(category, CancellationToken.None)).ReturnsAsync(eventsByCategory);
          

            var handler = new GetEventByCategoryHandler(_unitOfWorkMock.Object);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.events.Should().NotBeNullOrEmpty();
            result.events.Should().BeEquivalentTo(eventsByCategory);
        }

        [Fact]
        public async Task Handle_InvalidCategory_ThrowsNotFoundException()
        {
            // Arrange
            var invalidCategory = "InvalidCategory";
            var request = new GetEventByCategoryRequest ( invalidCategory );

            
            _unitOfWorkMock.Setup(u => u.EventRepository.GetByCategoryAsync(invalidCategory, CancellationToken.None)).ReturnsAsync((List<Event>)null);

            

            var handler = new GetEventByCategoryHandler(_unitOfWorkMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(request, CancellationToken.None));
        }
        
        [Fact]
        public async Task Handle_ReturnsEventsByDate()
        {
            // Arrange
            var date = DateTime.UtcNow.Date;
            var request = new GetEventByDateRequest (date );
            var eventsByDate = new List<Event>
            {
                new Event { Id = "1", Name = "Event 1", Date = date },
                new Event { Id = "2", Name = "Event 2", Date = date }
            };

            
            _unitOfWorkMock.Setup(u => u.EventRepository.GetByDateAsync(date, CancellationToken.None)).ReturnsAsync(eventsByDate);

            
            _mapperMock.Setup(m => m.Map<IEnumerable<Event>>(It.IsAny<IEnumerable<Event>>())).Returns<IEnumerable<Event>>(ev => ev);

            var handler = new GetEventByDateHandler(_unitOfWorkMock.Object);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.events.Should().NotBeNullOrEmpty();
            result.events.Should().BeEquivalentTo(eventsByDate);
        }

        [Fact]
        public async Task Handle_InvalidDate_ThrowsNotFoundException()
        {
            // Arrange
            var invalidDate = DateTime.UtcNow.AddDays(-1); 
            var request = new GetEventByDateRequest (invalidDate );

            
            _unitOfWorkMock.Setup(u => u.EventRepository.GetByDateAsync(invalidDate, CancellationToken.None)).ReturnsAsync((List<Event>)null);

            

            var handler = new GetEventByDateHandler(_unitOfWorkMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WithCategory_ReturnsEventsByCategory()
        {
            // Arrange
            
            var request = new GetFilteredEventsRequest
            {
                category = "Test Category"
            };

            var eventsByCategory = new List<Event> { new Event { Id = "1", Category = "Test Category" } };

            _unitOfWorkMock.Setup(uow => uow.EventRepository.GetByCategoryAsync(request.category, It.IsAny<CancellationToken>()))
                .ReturnsAsync(eventsByCategory);

            var handler = new GetFilteredEventsHandler(_unitOfWorkMock.Object);

            // Act
            var response = await handler.Handle(request, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.events.Should().NotBeNull();
            response.events.Should().ContainSingle();
            response.events.Should().BeEquivalentTo(eventsByCategory);
        }

        [Fact]
        public async Task Handle_WithPlace_ReturnsEventsByPlace()
        {
            // Arrange
            
            var request = new GetFilteredEventsRequest
            {
                place = "Test Place"
            };

            var eventsByPlace = new List<Event> { new Event { Id = "1", Place = "Test Place" } };

            _unitOfWorkMock.Setup(uow => uow.EventRepository.GetByPlaceAsync(request.place, It.IsAny<CancellationToken>()))
                .ReturnsAsync(eventsByPlace);

            var handler = new GetFilteredEventsHandler(_unitOfWorkMock.Object);

            // Act
            var response = await handler.Handle(request, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.events.Should().NotBeNull();
            response.events.Should().ContainSingle();
            response.events.Should().BeEquivalentTo(eventsByPlace);
        }

        [Fact]
        public async Task Handle_WithEmptyRequest_ReturnsEmptyEventsList()
        {
            // Arrange
            
            var request = new GetFilteredEventsRequest();

            var handler = new GetFilteredEventsHandler(_unitOfWorkMock.Object);

            // Act
            var response = await handler.Handle(request, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.events.Should().NotBeNull();
            response.events.Should().BeEmpty();
        }
        
        [Fact]
        public async Task Handle_WithValidRequest_ReturnsSuccessResponse()
        {
            // Arrange
            
            var request = new RegisterUserOnEventRequest
            {
                userId = "1",
                eventId = "1"
            };

            var eventEntity = new Event { Id = "1", PlacesLeft = 2 };
            var userExists = true;

            _unitOfWorkMock.Setup(uow => uow.EventRepository.GetByIdAsync(request.eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(eventEntity);
            _unitOfWorkMock.Setup(uow => uow.UserRepository.Exists(request.userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(userExists);
            _unitOfWorkMock.Setup(uow => uow.EventsUsersRepository.RegisterUserOnEvent(request.userId, request.eventId, It.IsAny<CancellationToken>()))
                .Verifiable();
            _unitOfWorkMock.Setup(uow => uow.EventRepository.UpdatePlacesLeftAsync(eventEntity, 1, It.IsAny<CancellationToken>()))
                .Verifiable();
            _unitOfWorkMock.Setup(uow => uow.Save())
                .Returns(Task.CompletedTask);

            var handler = new RegisterUserOnEventHandler(_unitOfWorkMock.Object);

            // Act
            var response = await handler.Handle(request, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Message.Should().Be("User successfully registered on event");
            _unitOfWorkMock.Verify(uow => uow.EventsUsersRepository.RegisterUserOnEvent(request.userId, request.eventId, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.EventRepository.UpdatePlacesLeftAsync(eventEntity, 1, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Save(), Times.Once);
        }

        [Fact]
        public async Task Handle_WithInvalidEventId_ThrowsNotFoundException()
        {
            // Arrange
            
            var request = new RegisterUserOnEventRequest
            {
                userId = "1",
                eventId = "1"
            };

            _unitOfWorkMock.Setup(uow => uow.EventRepository.GetByIdAsync(request.eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Event)null);

            var handler = new RegisterUserOnEventHandler(_unitOfWorkMock.Object);

            // Act
            Func<Task> act = async () => await handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_WithNoPlacesLeft_ThrowsArgumentException()
        {
            // Arrange
            
            var request = new RegisterUserOnEventRequest
            {
                userId = "1",
                eventId = "1"
            };

            var eventEntity = new Event { Id = "1", PlacesLeft = 0 };

            _unitOfWorkMock.Setup(uow => uow.EventRepository.GetByIdAsync(request.eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(eventEntity);

            var handler = new RegisterUserOnEventHandler(_unitOfWorkMock.Object);

            // Act
            Func<Task> act = async () => await handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task Handle_WithInvalidUserId_ThrowsNotFoundException()
        {
            // Arrange
            
            var request = new RegisterUserOnEventRequest
            {
                userId = "1",
                eventId = "1"
            };

            var eventEntity = new Event { Id = "1", PlacesLeft = 2 };
            var userExists = false;

            _unitOfWorkMock.Setup(uow => uow.EventRepository.GetByIdAsync(request.eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(eventEntity);
            _unitOfWorkMock.Setup(uow => uow.UserRepository.Exists(request.userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(userExists);

            var handler = new RegisterUserOnEventHandler(_unitOfWorkMock.Object);

            // Act
            Func<Task> act = async () => await handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
        
        [Fact]
        public async Task UnregisterHandle_WithValidRequest_ReturnsSuccessResponse()
        {
            // Arrange
            
            var request = new UnRegisterUserOnEventRequest
            {
                userId = "1",
                eventId = "1"
            };

            var eventEntity = new Event { Id = "1", PlacesLeft = 2 };
            var userExists = true;
            var unregisterResult = true;

            _unitOfWorkMock.Setup(uow => uow.EventRepository.GetByIdAsync(request.eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(eventEntity);
            _unitOfWorkMock.Setup(uow => uow.UserRepository.Exists(request.userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(userExists);
            _unitOfWorkMock.Setup(uow => uow.EventsUsersRepository.UnregisterUserOnEvent(request.userId, request.eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(unregisterResult);
            _unitOfWorkMock.Setup(uow => uow.EventRepository.UpdatePlacesLeftAsync(eventEntity, -1, It.IsAny<CancellationToken>()))
                .Verifiable();
            _unitOfWorkMock.Setup(uow => uow.Save())
                .Returns(Task.CompletedTask);

            var handler = new UnRegisterUserOnEventHandler(_unitOfWorkMock.Object);

            // Act
            var response = await handler.Handle(request, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Message.Should().Be("User successfully unregistered on event");
            _unitOfWorkMock.Verify(uow => uow.EventsUsersRepository.UnregisterUserOnEvent(request.userId, request.eventId, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.EventRepository.UpdatePlacesLeftAsync(eventEntity, -1, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Save(), Times.Once);
        }

        [Fact]
        public async Task UnregisterHandle_WithInvalidEventId_ThrowsNotFoundException()
        {
            // Arrange
            
            var request = new UnRegisterUserOnEventRequest
            {
                userId = "1",
                eventId = "1"
            };

            _unitOfWorkMock.Setup(uow => uow.EventRepository.GetByIdAsync(request.eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Event)null);

            var handler = new UnRegisterUserOnEventHandler(_unitOfWorkMock.Object);

            // Act
            Func<Task> act = async () => await handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task UnregisterHandle_WithInvalidUserId_ThrowsNotFoundException()
        {
            // Arrange
            
            var request = new UnRegisterUserOnEventRequest
            {
                userId = "1",
                eventId = "1"
            };

            var eventEntity = new Event { Id = "1", PlacesLeft = 2 };
            var userExists = false;

            _unitOfWorkMock.Setup(uow => uow.EventRepository.GetByIdAsync(request.eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(eventEntity);
            _unitOfWorkMock.Setup(uow => uow.UserRepository.Exists(request.userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(userExists);

            var handler = new UnRegisterUserOnEventHandler(_unitOfWorkMock.Object);

            // Act
            Func<Task> act = async () => await handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_WithUnregisterFailure_ThrowsNotFoundException()
        {
            // Arrange
            
            var request = new UnRegisterUserOnEventRequest
            {
                userId = "1",
                eventId = "1"
            };

            var eventEntity = new Event { Id = "1", PlacesLeft = 2 };
            var userExists = true;
            var unregisterResult = false;

            _unitOfWorkMock.Setup(uow => uow.EventRepository.GetByIdAsync(request.eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(eventEntity);
            _unitOfWorkMock.Setup(uow => uow.UserRepository.Exists(request.userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(userExists);
            _unitOfWorkMock.Setup(uow => uow.EventsUsersRepository.UnregisterUserOnEvent(request.userId, request.eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(unregisterResult);

            var handler = new UnRegisterUserOnEventHandler(_unitOfWorkMock.Object);

            // Act
            Func<Task> act = async () => await handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
        
        [Fact]
        public async Task UpdateHandle_WithValidRequest_ReturnsSuccessResponse()
        {
            // Arrange
            
            
            var request = new UpdateEventRequest
            {
                Id = "1",
            };
            var entity = new Event { Id = "1", };
            var response = new UpdateEventResponse { Id = "1", };

            _mapperMock.Setup(mapper => mapper.Map<Event>(request))
                .Returns(entity);
            _unitOfWorkMock.Setup(uow => uow.EventRepository.Exists(entity.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(uow => uow.EventRepository.UpdatePlacesLeftAsync(entity, 0, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(uow => uow.Save())
                .Returns(Task.CompletedTask);
            _mapperMock.Setup(mapper => mapper.Map<UpdateEventResponse>(entity))
                .Returns(response);

            var handler = new UpdateEventHandler(_unitOfWorkMock.Object, _mapperMock.Object);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(response);
            _unitOfWorkMock.Verify(uow => uow.EventRepository.Exists(entity.Id, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.EventRepository.UpdatePlacesLeftAsync(entity, 0, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Save(), Times.Once);
        }

        [Fact]
        public async Task UpdateHandle_WithInvalidEventId_ThrowsNotFoundException()
        {
            // Arrange
            
            
            var request = new UpdateEventRequest
            {
                Id = "1",
            };
            var entity = new Event { Id = "1", };

            _mapperMock.Setup(mapper => mapper.Map<Event>(request))
                .Returns(entity);
            _unitOfWorkMock.Setup(uow => uow.EventRepository.Exists(entity.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var handler = new UpdateEventHandler(_unitOfWorkMock.Object, _mapperMock.Object);

            // Act
            async Task Act() => await handler.Handle(request, CancellationToken.None);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WithUpdateFailure_ThrowsBadRequestException()
        {
            // Arrange
            
            
            var request = new UpdateEventRequest
            {
                Id = "1",
            };
            var entity = new Event { Id = "1", };

            _mapperMock.Setup(mapper => mapper.Map<Event>(request))
                .Returns(entity);
            _unitOfWorkMock.Setup(uow => uow.EventRepository.Exists(entity.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _unitOfWorkMock.Setup(uow => uow.EventRepository.UpdatePlacesLeftAsync(entity, 0, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var handler = new UpdateEventHandler(_unitOfWorkMock.Object, _mapperMock.Object);
            await Assert.ThrowsAsync<BadRequestException>(async () => await handler.Handle(request, CancellationToken.None));
            
        }
}