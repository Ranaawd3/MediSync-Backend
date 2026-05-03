using FluentValidation;
using MediSync.Application.DTOs.HealthMetrics;

namespace MediSync.Application.Validators;

public class HealthMetricValidator : AbstractValidator<AddHealthMetricDto>
{
    public HealthMetricValidator()
    {
        RuleFor(x => x.MetricType)
            .Must(t => t is "bp" or "sugar" or "weight")
            .WithMessage("metricType: bp | sugar | weight");

        RuleFor(x => x.Value)
            .GreaterThan(0)
            .WithMessage("value: لازم يكون أكبر من صفر");

        RuleFor(x => x.Unit)
            .Must(u => u is "mmHg" or "mg/dL" or "kg")
            .WithMessage("unit: mmHg | mg/dL | kg");

        // للضغط: SecondaryValue مطلوب
        RuleFor(x => x.SecondaryValue)
            .NotNull()
            .When(x => x.MetricType == "bp")
            .WithMessage("secondaryValue مطلوب للضغط (diastolic)");
    }
}