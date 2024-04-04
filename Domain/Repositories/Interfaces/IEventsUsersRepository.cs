namespace Domain.Repositories.Interfaces;

public interface IEventsUsersRepository
{
    Task RegisterUserOnEvent(string userId, string eventId);
    Task UnregisterUserOnEvent(string userId, string eventId);

}