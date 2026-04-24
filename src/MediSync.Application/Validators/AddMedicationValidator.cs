using FluentValidation;
using MediSync.Application.DTOs.Medications;
using MediSync.Domain.Enums;

namespace MediSync.Application.Validators;

public class AddMedicationValidator : AbstractValidator<AddMedicationDto>
{
    public AddMedicationValidator()
    {
        RuleFor(x => x.BrandName)
            .NotEmpty().WithMessage("اسم الدواء مطلوب")
            .MaximumLength(200);

        RuleFor(x => x.ActiveIngredient)
            .NotEmpty().WithMessage("المادة الفعالة مطلوبة")
            .MaximumLength(200);

        RuleFor(x => x.DosageValue)
            .GreaterThan(0).WithMessage("dosageValue: لازم يكون أكبر من صفر");

        RuleFor(x => x.DosageUnit)
            .Must(u => u is "mg" or "ml" or "IU" or "mcg")
            .WithMessage("dosageUnit: mg | ml | IU | mcg");

        RuleFor(x => x.Form)
            .Must(f => f is "tablet" or "capsule" or "syrup" or "injection" or "drops");

        RuleFor(x => x.TimesPerDay)
            .InclusiveBetween(1, 10);

        RuleFor(x => x.ScheduleType)
            .Must(s => s is "empty_stomach" or "with_food"
                          or "after_food"    or "before_sleep");

        RuleFor(x => x.ColorCode)
            .Must(c => c is "red" or "yellow" or "green" or "blue");

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .Matches(@"^\d{4}-\d{2}-\d{2}$")
            .WithMessage("startDate: YYYY-MM-DD");
    }
}