using MediSync.Application.DTOs.Medications;
using MediSync.Application.DTOs.Interactions;
using MediSync.Application.DTOs.Reminders;
using MediSync.Application.DTOs.HealthMetrics;

namespace MediSync.Application.DTOs.Reports;

// متوافق مع Contract Section 7 — Report Variables
public record MonthlyReportDto(
    Guid                       ReportId,
    string                     PatientName,
    string                     GeneratedAt,   // ISO 8601
    string                     PeriodStart,   // YYYY-MM-DD
    string                     PeriodEnd,     // YYYY-MM-DD
    List<MedicationDto>         Medications,
    List<InteractionResultDto>  Interactions,
    AdherenceDto               AdherenceStats,
    List<HealthMetricDto>      HealthMetrics,
    string?                    PdfUrl         // رابط الـ PDF لو اتعمل
);