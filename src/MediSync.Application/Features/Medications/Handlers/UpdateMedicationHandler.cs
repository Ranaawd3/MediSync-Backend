using MediatR;
using MediSync.Application.DTOs.Medications;
using MediSync.Application.Features.Medications.Commands;
using MediSync.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediSync.Application.Features.Medications.Handlers;

public class UpdateMedicationHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateMedicationCommand, MedicationDto>
{
    public async Task<MedicationDto> Handle(
        UpdateMedicationCommand req, CancellationToken ct)
    {
        var m = await db.Medications
            .FirstOrDefaultAsync(x => x.Id == req.MedicationId
                                   && x.UserId == req.UserId
                                   && x.IsActive, ct)
            ?? throw new KeyNotFoundException("MEDICATION_NOT_FOUND");

        var dto = req.Dto;
        if (dto.BrandName    != null) m.BrandName    = dto.BrandName;
        if (dto.GenericName  != null) m.GenericName  = dto.GenericName;
        if (dto.DosageValue  != null) m.DosageValue  = dto.DosageValue.Value;
        if (dto.DosageUnit   != null) m.DosageUnit   = dto.DosageUnit;
        if (dto.TimesPerDay  != null) m.TimesPerDay  = dto.TimesPerDay.Value;
        if (dto.ScheduleType != null) m.ScheduleType = dto.ScheduleType;
        if (dto.DurationDays != null) m.DurationDays = dto.DurationDays;
        if (dto.StockCount   != null) m.StockCount   = dto.StockCount;
        if (dto.ColorCode    != null) m.ColorCode    = dto.ColorCode;

        await db.SaveChangesAsync(ct);

        return new MedicationDto(
            m.Id, m.UserId, m.BrandName, m.GenericName, m.ActiveIngredient,
            m.DosageValue, m.DosageUnit, m.Form, m.TimesPerDay, m.ScheduleType,
            m.DurationDays, m.StartDate.ToString("yyyy-MM-dd"),
            m.EndDate?.ToString("yyyy-MM-dd"), m.StockCount, m.ColorCode,
            m.Source, m.IsActive, m.CreatedAt.ToString("o"));
    }
}