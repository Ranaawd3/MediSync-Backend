using MediatR;
using MediSync.Application.DTOs.Reports;
using MediSync.Application.DTOs.Medications;
using MediSync.Application.DTOs.Interactions;
using MediSync.Application.DTOs.Reminders;
using MediSync.Application.DTOs.HealthMetrics;
using MediSync.Application.Features.Reports.Queries;
using MediSync.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediSync.Application.Features.Reports.Handlers;

public class GetMonthlyReportHandler(IApplicationDbContext db)
    : IRequestHandler<GetMonthlyReportQuery, MonthlyReportDto>
{
    public async Task<MonthlyReportDto> Handle(
        GetMonthlyReportQuery req, CancellationToken ct)
    {
        var user  = await db.Users.FindAsync(req.UserId, ct)
            ?? throw new KeyNotFoundException("المستخدم غير موجود");

        var start = new DateOnly(req.Year, req.Month, 1);
        var end   = start.AddMonths(1).AddDays(-1);

        // ─── 1. أدوية المستخدم النشطة ───────────────────────
        var medications = await db.Medications
            .Where(m => m.UserId == req.UserId && m.IsActive)
            .Select(m => new MedicationDto(
                m.Id, m.UserId, m.BrandName, m.GenericName, m.ActiveIngredient,
                m.DosageValue, m.DosageUnit, m.Form, m.TimesPerDay, m.ScheduleType,
                m.DurationDays, m.StartDate.ToString("yyyy-MM-dd"),
                m.EndDate == null ? null : m.EndDate.Value.ToString("yyyy-MM-dd"),
                m.StockCount, m.ColorCode, m.Source, m.IsActive,
                m.CreatedAt.ToString("o")))
            .ToListAsync(ct);

        // ─── 2. التفاعلات الدوائية النشطة ───────────────────
            var ingredients = medications
                .Select(m => m.ActiveIngredient.ToLower())
                .Distinct()
                .ToList();

            var interactions_raw = await db.DrugInteractions
                .Where(d => ingredients.Contains(d.Drug1Ingredient)
                        && ingredients.Contains(d.Drug2Ingredient))
                .ToListAsync(ct);

            var interactions = interactions_raw.Select(d => new InteractionResultDto(
                 d.Drug1Ingredient, d.Drug2Ingredient,
                 d.Drug1Ingredient, d.Drug2Ingredient,
                 d.Severity, d.DescriptionAr, d.DescriptionEn,
                 d.Mechanism, d.Alternatives, new List<string>())).ToList();

        // ─── 3. الالتزام بالأدوية للشهر ────────────────────
        var reminders = await db.Reminders
            .Where(r => r.UserId        == req.UserId
                     && r.ScheduledDate >= start
                     && r.ScheduledDate <= end)
            .ToListAsync(ct);

        var total  = reminders.Count;
        var taken  = reminders.Count(r => r.Status == "taken");
        var missed = reminders.Count(r => r.Status == "missed");
        var pct    = total > 0
            ? Math.Round((decimal)taken / total * 100, 1)
            : 0m;

        var adherence = new AdherenceDto(
            req.UserId, "month", total, taken, missed, pct, 0, 0);

        // ─── 4. القياسات الصحية للشهر ───────────────────────
        var startDt = start.ToDateTime(TimeOnly.MinValue);
        var endDt   = end.ToDateTime(TimeOnly.MaxValue);

        var healthMetrics = await db.HealthMetrics
            .Where(h => h.UserId     == req.UserId
                     && h.RecordedAt >= startDt
                     && h.RecordedAt <= endDt)
            .OrderBy(h => h.RecordedAt)
            .Select(h => new HealthMetricDto(
                h.Id, h.UserId, h.MetricType,
                h.Value, h.SecondaryValue, h.Unit,
                h.Notes, h.RecordedAt.ToString("o")))
            .ToListAsync(ct);

        return new MonthlyReportDto(
            Guid.NewGuid(),
            user.FullName,
            DateTime.UtcNow.ToString("o"),
            start.ToString("yyyy-MM-dd"),
            end.ToString("yyyy-MM-dd"),
            medications,
            interactions,
            adherence,
            healthMetrics,
            null   // pdfUrl — هيتعمل في QuestPdfService
        );
    }
}