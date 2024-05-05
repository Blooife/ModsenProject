using FluentValidation;

namespace Application.UseCases.AuthUseCases.Register;

public class RegisterValidator: AbstractValidator<RegisterRequest>
{
    public RegisterValidator()
    {
        RuleFor(u => u.Name).NotEmpty().MinimumLength(2).MaximumLength(30);
        RuleFor(u => u.LastName).NotEmpty().MinimumLength(2).MaximumLength(30);
        RuleFor(u => u.BirthDate).NotEmpty().LessThan(DateTime.Now);
    }
}