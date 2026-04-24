using MediatR;
using MediSync.Application.DTOs.Medications;
using MediSync.Application.Features.Medications.Queries;
using MediSync.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediSync.Application.Features.Medications.Handlers;

public class GetMedicationByIdHandler(IApplicationDbContext db)
    : IRequestHandler<GetMedicationByIdQuery, MedicationDto>
{
    public async Task<MedicationDto> Handle(
        GetMedicationByIdQuery req, CancellationToken ct)
    {
        var m = await db.Medications
            .FirstOrDefaultAsync(x => x.Id == req.MedicationId
                                   && x.UserId == req.UserId
                                   && x.IsActive, ct)
            ?? throw new KeyNotFoundException("MEDICATION_NOT_FOUND");

        return new MedicationDto(
            m.Id, m.UserId, m.BrandName, m.GenericName, m.ActiveIngredient,
            m.DosageValue, m.DosageUnit, m.Form, m.TimesPerDay, m.ScheduleType,
            m.DurationDays, m.StartDate.ToString("yyyy-MM-dd"),
            m.EndDate?.ToString("yyyy-MM-dd"), m.StockCount, m.ColorCode,
            m.Source, m.IsActive, m.CreatedAt.ToString("o"));
    }
}