using MediatR;

namespace Application.UseCases.EventUseCases.GetFilteredEvents;

public sealed record GetFilteredEventsRequest: IRequest<GetFilteredEventsResponse>
{
    public string? category { get; set; }
    public string? place { get; set; }
}