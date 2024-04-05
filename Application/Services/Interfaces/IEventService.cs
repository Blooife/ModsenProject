using Application.Models.Dtos;
using Domain.Models.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Servicies.Interfaces;

public interface IEventService
{
    Task<IEnumerable<Event>> GetAllAsync(CancellationToken cancellationToken);

    Task<Event> GetByIdAsync(string id, CancellationToken cancellationToken);

    Task<Event> CreateAsync(EventRequestDto dto, CancellationToken cancellationToken);

    Task<Event> UpdateAsync(EventRequestDto dto, CancellationToken cancellationToken);

    Task<Event> DeleteAsync(string id, CancellationToken cancellationToken);
    
    Task<IEnumerable<Event>> GetByCategoryAsync(string category, CancellationToken cancellationToken);
    Task<IEnumerable<Event>> GetByPlaceAsync(string place, CancellationToken cancellationToken);
    Task<IEnumerable<Event>> GetByDateAsync(DateTime date, CancellationToken cancellationToken);
    Task<Event> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task<IEnumerable<Event>> GetFilteredEventsAsync(FiltersRequestDto filters, CancellationToken cancellationToken);
    Task<ResponseDto> UploadPictureAsync(IFormFile file, string eventId, CancellationToken cancellationToken);
    Task<ResponseDto> RegisterUserOnEvent(string userId, string eventId, CancellationToken cancellationToken);
    Task<ResponseDto> UnregisterUserOnEvent(string userId, string eventId, CancellationToken cancellationToken);
    Task<IEnumerable<Event>> GetAllUserEvents(string userId, CancellationToken cancellationToken);
    Task<PagedList<Event>> GetPagedEvents(PageParams pageParams, CancellationToken cancellationToken);
}