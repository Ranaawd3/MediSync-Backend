using FluentValidation;
using MediSync.Application.DTOs.Auth;

namespace MediSync.Application.Validators;

public class RegisterValidator : AbstractValidator<RegisterDto>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email)   .NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Age)     .InclusiveBetween(1, 120);
        RuleFor(x => x.Gender)  .Must(g => g is "male" or "female" or "other");
    }
}