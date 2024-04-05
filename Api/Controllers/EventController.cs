using Application.Models.Dtos;
using Application.Servicies.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/events")]
public class EventController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventController(IEventService eventService)
    {
        _eventService = eventService;
    }
    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetEvents(CancellationToken cancellationToken)
    {
        var events = await _eventService.GetAllAsync(cancellationToken);
        return Ok(events);
    }
    
    [HttpPost("getPagedEvents")]
    [Authorize]
    public async Task<IActionResult> GetPagedEvents([FromBody] PageParams pageParams, CancellationToken cancellationToken)
    {
        var events = await _eventService.GetPagedEvents(pageParams, cancellationToken);
        var metadata = new
        {
            events.TotalCount,
            events.PageSize,
            events.CurrentPage,
            events.TotalPages,
            events.HasNext,
            events.HasPrevious
        };

        HttpContext.Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
        return Ok(events);
    }
    
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetEventById(string id, CancellationToken cancellationToken)
    {
        var ev = await _eventService.GetByIdAsync(id, cancellationToken);
        return Ok(ev);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateEvent([FromBody] EventRequestDto post, CancellationToken cancellationToken)
    {
        var createdEvent = await _eventService.CreateAsync(post, cancellationToken);
        return CreatedAtAction(nameof(CreateEvent), new { id = createdEvent.Id }, createdEvent);
    }

    [HttpPut]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> UpdateEvent([FromBody] EventRequestDto post, CancellationToken cancellationToken)
    {
        var updatedEvent = await _eventService.UpdateAsync(post, cancellationToken);
        return Ok(updatedEvent);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeleteEvent(string id, CancellationToken cancellationToken)
    {
        await _eventService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
    
    [HttpGet("getByName/{name}")]
    [Authorize]
    public async Task<IActionResult> GetEventByName(string name, CancellationToken cancellationToken)
    {
        var ev = await _eventService.GetByNameAsync(name, cancellationToken);
        return Ok(ev);
    }
    
    [HttpGet("getByCategory/{category}")]
    [Authorize]
    public async Task<IActionResult> GetEventsByCategory(string category, CancellationToken cancellationToken)
    {
        var ev = await _eventService.GetByCategoryAsync(category, cancellationToken);
        return Ok(ev);
    }
    
    [HttpGet("getByPlace/{place}")]
    [Authorize]
    public async Task<IActionResult> GetEventsByPlace(string place, CancellationToken cancellationToken)
    {
        var ev = await _eventService.GetByPlaceAsync(place, cancellationToken);
        return Ok(ev);
    }
    
    [HttpGet("getByDate/{date}")]
    [Authorize]
    public async Task<IActionResult> GetEventsByDate(DateTime date, CancellationToken cancellationToken)
    {
        var ev = await _eventService.GetByDateAsync(date, cancellationToken);
        return Ok(ev);
    }
    
    [HttpPost("getFilteredEvents")]
    [Authorize]
    public async Task<IActionResult> GetFilteredEvents([FromBody] FiltersRequestDto filters, CancellationToken cancellationToken)
    {
        var events = await _eventService.GetFilteredEventsAsync(filters, cancellationToken);
        return Ok(events);
    }
    
    [HttpPost("upload")]
    [Authorize(Roles = "ADMIN")]
    public async Task<object> UploadPicture([FromForm] IFormFile file, [FromForm]string eventId, CancellationToken cancellationToken)
    {
        var response = await _eventService.UploadPictureAsync(file, eventId, cancellationToken);
        return Ok(response.Message);
    }
    
    [HttpPost("registerOnEvent")]
    [Authorize]
    public async Task<IActionResult> RegisterUserOnEvent([FromBody] EventUserDto model, CancellationToken cancellationToken)
    {
        var response = await _eventService.RegisterUserOnEvent(model.userId, model.eventId, cancellationToken);

        return Ok(response);
    }
    
    [HttpPost("unRegisterOnEvent")]
    [Authorize]
    public async Task<IActionResult> UnRegisterUserOnEvent([FromBody] EventUserDto model, CancellationToken cancellationToken)
    {
        var response = await _eventService.UnregisterUserOnEvent(model.userId, model.eventId, cancellationToken);

        return Ok(response);
    }
    
    [HttpGet("getEvents/{id}")]
    [Authorize]
    public async Task<IActionResult> GetAllUserEvents(string id, CancellationToken cancellationToken)
    {
        var response = await _eventService.GetAllUserEvents(id, cancellationToken);

        return Ok(response);
    }
}