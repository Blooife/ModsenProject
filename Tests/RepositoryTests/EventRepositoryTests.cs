using AutoMapper;
using Domain.Models.Entities;
using Domain.Repositories.Interfaces;
using EFCore.Toolkit.Testing;
using Infrastructure.Data;
using Infrastructure.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;

namespace Tests.RepositoryTests;

public class EventRepositoryTests
{
    private readonly Mock<AppDbContext> _dbContextMock;
    private readonly EventRepository _eventRepository;

    public EventRepositoryTests()
    {
        _dbContextMock = new Mock<AppDbContext>();
        _eventRepository = new EventRepository(_dbContextMock.Object);
    }

    [Fact]
    public async Task GetByCategoryAsync_ReturnsMatchingEvents()
    {
        // Arrange
        string category = "TestCategory";
        List<Event> events = new List<Event>
        {
            new Event { Id = "1", Category = category },
            new Event { Id = "2", Category = "OtherCategory" }
        };

        _dbContextMock.Setup(x => x.Events)
            .Returns(MockDbSet(events).Object);

        // Act
        var result = await _eventRepository.GetByCategoryAsync(category, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(category, result.First().Category);
    }
    
    [Fact]
    public async Task GetByPlaceAsync_ReturnsMatchingEvents()
    {
        // Arrange
        string place = "TestPlace";
        List<Event> events = new List<Event>
        {
            new Event { Id = "1", Place = place },
            new Event { Id = "2", Place = "OtherPlace" }
        };

        _dbContextMock.Setup(x => x.Events)
            .Returns(MockDbSet(events).Object);

        // Act
        var result = await _eventRepository.GetByPlaceAsync(place, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(place, result.First().Place);
    }

    [Fact]
    public async Task GetByDateAsync_ReturnsMatchingEvents()
    {
        // Arrange
        DateTime date = DateTime.UtcNow.Date;
        List<Event> events = new List<Event>
        {
            new Event { Id = "1", Date = date },
            new Event { Id = "2", Date = date.AddDays(-1) }
        };

        _dbContextMock.Setup(x => x.Events)
            .Returns(MockDbSet(events).Object);

        // Act
        var result = await _eventRepository.GetByDateAsync(date, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(date, result.First().Date);
    }
    
    [Fact]
    public async Task GetByNameAsync_ReturnsMatchingEvent()
    {
        // Arrange
        string name = "TestEvent";
        List<Event> events = new List<Event>
        {
            new Event { Id = "1", Name = name },
            new Event { Id = "2", Name = "OtherEvent" }
        };

        _dbContextMock.Setup(x => x.Events)
            .Returns(MockDbSet(events).Object);

        // Act
        var result = await _eventRepository.GetByNameAsync(name, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
    }
    
    [Fact]
        public async Task GetByIdAsync_ReturnsEvent()
        {
            // Arrange
            string id = "-1";
            Event expectedEvent = new Event { Id = id, Name = "Test Event" };

            _dbContextMock.Setup(x => x.Set<Event>().FindAsync(id, CancellationToken.None))
                .ReturnsAsync(expectedEvent);

            // Act
            var result = await _eventRepository.GetByIdAsync(id, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedEvent, result);
        }

        private Mock<DbSet<T>> MockDbSet<T>(List<T> data) where T : class
        {
            var queryableData = data.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator(default))
                .Returns(new MockQueryable.EntityFrameworkCore.TestAsyncEnumerator<T>(queryableData.GetEnumerator()));

            mockSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<T>(queryableData.Provider));
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryableData.GetEnumerator());

            return mockSet;
        }
}