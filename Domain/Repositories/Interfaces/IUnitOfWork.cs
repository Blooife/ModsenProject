namespace Domain.Repositories.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IEventRepository EventRepository { get; }
    IUserRepository UserRepository { get; }
    
    IEventsUsersRepository EventsUsersRepository { get; }

    Task Save();
}