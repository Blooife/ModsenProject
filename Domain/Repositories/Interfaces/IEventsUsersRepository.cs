using Domain.Models.Entities;

namespace Domain.Repositories.Interfaces;

public interface IEventsUsersRepository
{
    Task RegisterUserOnEvent(string userId, string eventId, CancellationToken cancellationToken);
    Task<bool> UnregisterUserOnEvent(string userId, string eventId, CancellationToken cancellationToken);
    Task<IEnumerable<Event>> GetAllUserEvents(string userId, CancellationToken cancellationToken);

}