using Domain.Models.Entities;

namespace Domain.Repositories.Interfaces;

public interface IEventRepository : IGenericRepository<Event>
{
    Task<IEnumerable<Event>> GetByCategoryAsync(string category);
    Task<IEnumerable<Event>> GetByPlaceAsync(string place);
    Task<IEnumerable<Event>> GetByDateAsync(DateTime date);
    Task<Event?> GetByNameAsync(string name);
    Task UpdatePlacesLeftAsync(Event entity, int inc);
}