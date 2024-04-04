using Application.Models.Dtos;
using Domain.Models.Entities;
using FluentValidation;

namespace Application.Validators;

public class EventValidator : AbstractValidator<EventRequestDto>
{
    public EventValidator()
    {
        RuleFor(ev => ev.Name).MinimumLength(2).MaximumLength(50).NotEmpty();
        RuleFor(ev => ev.Place).MinimumLength(2).MaximumLength(50).NotEmpty();
        RuleFor(ev => ev.Category).MinimumLength(2).MaximumLength(30).NotEmpty();
        RuleFor(ev => ev.Description).MinimumLength(2).MaximumLength(300).NotEmpty();
        RuleFor(ev => ev.MaxParticipants).NotEmpty().GreaterThan(0);
        RuleFor(ev => ev.Date).NotEmpty().GreaterThan(DateTime.Now);
    }
}