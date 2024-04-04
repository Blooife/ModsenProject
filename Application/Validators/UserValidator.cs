using Application.Models.Dtos;
using FluentValidation;

namespace Application.Validators;

public class UserValidator : AbstractValidator<RegistrationRequestDto>
{
    public UserValidator()
    {
        RuleFor(u => u.Name).NotEmpty().MinimumLength(2).MaximumLength(30);
        RuleFor(u => u.LastName).NotEmpty().MinimumLength(2).MaximumLength(30);
        RuleFor(u => u.BirthDate).NotEmpty().LessThan(DateTime.Now);
    }
}