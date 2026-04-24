using MediatR;
using MediSync.Application.DTOs.Common;
using MediSync.Application.DTOs.Medications;
using MediSync.Application.Features.Medications.Commands;
using MediSync.Domain.Constants;
using MediSync.Domain.Entities;
using MediSync.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediSync.Application.Features.Medications.Handlers;

public class AddMedicationHandler(IApplicationDbContext db, IMediator mediator)
    : IRequestHandler<AddMedicationCommand, AddMedicationResult>
{
    public async Task<AddMedicationResult> Handle(
        AddMedicationCommand req, CancellationToken ct)
    {
        var dto = req.Dto;

        // 1. تحقق من الحساسية
        var user = await db.Users.FindAsync(req.UserId, ct)
            ?? throw new KeyNotFoundException("المستخدم غير موجود");

        var warnings = new List<WarningDto>();

        if (user.Allergies.Any(a =>
            a.ToLower().Contains(dto.ActiveIngredient.ToLower())))
        {
            warnings.Add(new WarningDto(
                ErrorCodes.AllergyDetected,
                "HIGH",
                $"تحذير: الدواء يحتوي على مادة من حساسياتك — {dto.ActiveIngredient}"
            ));
        }

        // 2. أضف الدواء
        var medication = new Medication
        {
            UserId          = req.UserId,
            BrandName       = dto.BrandName,
            GenericName     = dto.GenericName      ?? "",
            ActiveIngredient= dto.ActiveIngredient,
            DosageValue     = dto.DosageValue,
            DosageUnit      = dto.DosageUnit,
            Form            = dto.Form,
            TimesPerDay     = dto.TimesPerDay,
            ScheduleType    = dto.ScheduleType,
            DurationDays    = dto.DurationDays,
            StartDate       = DateOnly.Parse(dto.StartDate),
            StockCount      = dto.StockCount,
            ColorCode       = dto.ColorCode,
            Source          = dto.Source,
            IsActive        = true,
        };

        db.Medications.Add(medication);
        await db.SaveChangesAsync(ct);

        // 3. فحص التفاعلات تلقائياً
        var existingIngredients = await db.Medications
            .Where(m => m.UserId == req.UserId && m.IsActive && m.Id != medication.Id)
            .Select(m => m.ActiveIngredient.ToLower())
            .ToListAsync(ct);

        var newIngredient = dto.ActiveIngredient.ToLower();

        foreach (var existing in existingIngredients)
        {
            var interaction = await db.DrugInteractions.FirstOrDefaultAsync(
                d => (d.Drug1Ingredient == newIngredient && d.Drug2Ingredient == existing) ||
                     (d.Drug1Ingredient == existing     && d.Drug2Ingredient == newIngredient),
                ct);

            if (interaction != null)
            {
                warnings.Add(new WarningDto(
                    ErrorCodes.InteractionDetected,
                    interaction.Severity,
                    interaction.DescriptionAr
                ));
            }
        }

        var medicationDto = ToDto(medication);
        return new AddMedicationResult(medicationDto, warnings.Any(), warnings);
    }

    private static MedicationDto ToDto(Medication m) => new(
        m.Id, m.UserId, m.BrandName, m.GenericName, m.ActiveIngredient,
        m.DosageValue, m.DosageUnit, m.Form, m.TimesPerDay, m.ScheduleType,
        m.DurationDays, m.StartDate.ToString("yyyy-MM-dd"),
        m.EndDate?.ToString("yyyy-MM-dd"), m.StockCount, m.ColorCode,
        m.Source, m.IsActive, m.CreatedAt.ToString("o"));
}