using MediatR;
using MediSync.Application.DTOs.Common;
using MediSync.Application.DTOs.Family;
using MediSync.Application.Features.Family.Queries;
using MediSync.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediSync.Application.Features.Family.Handlers;

public class GetCaregiverDashboardHandler(
    IApplicationDbContext db
) : IRequestHandler<GetCaregiverDashboardQuery, ApiResponse<CaregiverDashboardDto>>
{
    public async Task<ApiResponse<CaregiverDashboardDto>> Handle(
        GetCaregiverDashboardQuery request,
        CancellationToken          ct)
    {
        // تحقق من وجود ربط نشط بين الـ Caregiver والـ Patient
        var link = await db.FamilyLinks
            .Include(f => f.Patient)
            .FirstOrDefaultAsync(
                f => f.PatientId    == request.PatientId
                  && f.CaregiverId  == request.CaregiverId
                  && f.Status       == "active", ct)
            ?? throw new UnauthorizedAccessException("AUTH_FORBIDDEN");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        // تذكيرات اليوم
        var reminders = await db.Reminders
            .Include(r => r.Medication)
            .Where(r => r.UserId == request.PatientId && r.ScheduledDate == today)
            .OrderBy(r => r.ScheduledTime)
            .ToListAsync(ct);

        // احسب الإحصائيات
        var total   = reminders.Count;
        var taken   = reminders.Count(r => r.Status == "taken");
        var missed  = reminders.Count(r => r.Status == "missed");
        var pending = reminders.Count(r => r.Status == "pending");

        // نسبة الالتزام آخر 7 أيام
        var weekAgo  = today.AddDays(-7);
        var weekRems = await db.Reminders
            .Where(r => r.UserId == request.PatientId
                     && r.ScheduledDate >= weekAgo
                     && r.ScheduledDate < today)
            .ToListAsync(ct);

        var adherence = weekRems.Count > 0
            ? weekRems.Count(r => r.Status == "taken") * 100.0 / weekRems.Count
            : 100.0;

        var reminderDtos = reminders.Select(r => new ReminderSummaryDto(
            r.Id,
            r.Medication.BrandName,
            $"{r.Medication.DosageValue}{r.Medication.DosageUnit}",
            r.ScheduledTime.ToString("HH:mm"),
            r.Status));

        var dto = new CaregiverDashboardDto(
            request.PatientId,
            link.Patient.FullName,
            total, taken, missed, pending,
            Math.Round(adherence, 1),
            reminderDtos);

        return ApiResponse<CaregiverDashboardDto>.Ok(dto);
    }
}