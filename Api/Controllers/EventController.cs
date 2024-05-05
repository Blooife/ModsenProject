using Application.Models.Dtos;
using Application.Servicies.Interfaces;
using Application.UseCases.EventUseCases.CreateEvent;
using Application.UseCases.EventUseCases.DeleteEvent;
using Application.UseCases.EventUseCases.GetAllEvents;
using Application.UseCases.EventUseCases.GetAllUserEvents;
using Application.UseCases.EventUseCases.GetEventByCategory;
using Application.UseCases.EventUseCases.GetEventByDate;
using Application.UseCases.EventUseCases.GetEventById;
using Application.UseCases.EventUseCases.GetEventByName;
using Application.UseCases.EventUseCases.GetEventByPlace;
using Application.UseCases.EventUseCases.GetFilteredEvents;
using Application.UseCases.EventUseCases.GetPagedEvents;
using Application.UseCases.EventUseCases.RegisterUserOnEvent;
using Application.UseCases.EventUseCases.UnRegisterUserOnEvent;
using Application.UseCases.EventUseCases.UpdateEvent;
using Application.UseCases.EventUseCases.UploadPicture;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/events")]
public class EventController : ControllerBase
{
    private readonly IMediator _mediator;

    public EventController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetEvents(CancellationToken cancellationToken)
    {
        var events = await _mediator.Send(new GetAllEventsRequest(), cancellationToken);
        return Ok(events.Events);
    }
    
    [HttpPost("getPagedEvents")]
    [Authorize]
    public async Task<IActionResult> GetPagedEvents([FromBody] GetPagedEventsRequest pageParams, CancellationToken cancellationToken)
    {
        var res = await _mediator.Send(pageParams, cancellationToken);
        var metadata = new
        {
            res.events.TotalCount,
            res.events.PageSize,
            res.events.CurrentPage,
            res.events.TotalPages,
            res.events.HasNext,
            res.events.HasPrevious
        };

        HttpContext.Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
        return Ok(res.events);
    }
    
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetEventById(string id, CancellationToken cancellationToken)
    {
        var ev = await _mediator.Send(new GetEventByIdRequest(id), cancellationToken);
        return Ok(ev);
    }
    
    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest post, CancellationToken cancellationToken)
    {
        var createdEvent = await _mediator.Send(post, cancellationToken);
        return CreatedAtAction(nameof(CreateEvent), new { id = createdEvent.Id }, createdEvent);
    }

    [HttpPut]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> UpdateEvent([FromBody] UpdateEventRequest post, CancellationToken cancellationToken)
    {
        var updatedEvent = await _mediator.Send(post, cancellationToken);
        return Ok(updatedEvent);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeleteEvent(string id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteEventRequest(id), cancellationToken);
        return NoContent();
    }
    
    [HttpGet("getByName/{name}")]
    [Authorize]
    public async Task<IActionResult> GetEventByName(string name, CancellationToken cancellationToken)
    {
        var ev = await _mediator.Send(new GetEventByNameRequest(name), cancellationToken);
        return Ok(ev);
    }
    
    [HttpGet("getByCategory/{category}")]
    [Authorize]
    public async Task<IActionResult> GetEventsByCategory(string category, CancellationToken cancellationToken)
    {
        var ev = await _mediator.Send(new GetEventByCategoryRequest(category), cancellationToken);
        return Ok(ev);
    }
    
    [HttpGet("getByPlace/{place}")]
    [Authorize]
    public async Task<IActionResult> GetEventsByPlace(string place, CancellationToken cancellationToken)
    {
        var ev = await _mediator.Send(new GetEventByPlaceRequest(place), cancellationToken);
        return Ok(ev);
    }
    
    [HttpGet("getByDate/{date}")]
    [Authorize]
    public async Task<IActionResult> GetEventsByDate(GetEventByDateRequest date, CancellationToken cancellationToken)
    {
        var ev = await _mediator.Send(date, cancellationToken);
        return Ok(ev);
    }
    
    [HttpPost("getFilteredEvents")]
    [Authorize]
    public async Task<IActionResult> GetFilteredEvents([FromBody] GetFilteredEventsRequest filters, CancellationToken cancellationToken)
    {
        var events = await _mediator.Send(filters, cancellationToken);
        return Ok(events.events);
    }
    
    [HttpPost("upload")]
    [Authorize(Roles = "ADMIN")]
    public async Task<object> UploadPicture([FromForm] IFormFile file, [FromForm]string eventId, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new UploadPictureRequest(file, eventId), cancellationToken);
        return Ok(response.Message);
    }
    
    [HttpPost("registerOnEvent")]
    [Authorize]
    public async Task<IActionResult> RegisterUserOnEvent([FromBody] RegisterUserOnEventRequest model, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(model, cancellationToken);

        return Ok(response);
    }
    
    [HttpPost("unRegisterOnEvent")]
    [Authorize]
    public async Task<IActionResult> UnRegisterUserOnEvent([FromBody] UnRegisterUserOnEventRequest model, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(model, cancellationToken);

        return Ok(response);
    }
    
    [HttpGet("getEvents/{id}")]
    [Authorize]
    public async Task<IActionResult> GetAllUserEvents(string id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetAllUserEventsRequest(id), cancellationToken);

        return Ok(response.events);
    }
}