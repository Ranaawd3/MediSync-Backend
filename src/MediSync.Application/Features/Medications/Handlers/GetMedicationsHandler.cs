using MediatR;
using MediSync.Application.DTOs.Medications;
using MediSync.Application.Features.Medications.Queries;
using MediSync.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediSync.Application.Features.Medications.Handlers;

public class GetMedicationsHandler(IApplicationDbContext db)
    : IRequestHandler<GetMedicationsQuery, List<MedicationDto>>
{
    public async Task<List<MedicationDto>> Handle(
        GetMedicationsQuery req, CancellationToken ct)
    {
        return await db.Medications
            .Where(m => m.UserId == req.UserId && m.IsActive)
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => new MedicationDto(
                m.Id, m.UserId, m.BrandName, m.GenericName, m.ActiveIngredient,
                m.DosageValue, m.DosageUnit, m.Form, m.TimesPerDay, m.ScheduleType,
                m.DurationDays, m.StartDate.ToString("yyyy-MM-dd"),
                m.EndDate == null ? null : m.EndDate.Value.ToString("yyyy-MM-dd"),
                m.StockCount, m.ColorCode, m.Source, m.IsActive,
                m.CreatedAt.ToString("o")))
            .ToListAsync(ct);
    }
}