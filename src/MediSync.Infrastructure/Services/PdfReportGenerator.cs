using MediSync.Application.DTOs.Reports;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MediSync.Infrastructure.Services;

public static class PdfReportGenerator
{
    public static byte[] Generate(MonthlyReportDto report)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11));

                // Header
                page.Header().Element(header =>
                {
                    header.Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("MediSync 💊")
                                .FontSize(20).Bold().FontColor("#1a56a0");
                            col.Item().Text("تقرير الأدوية الشهري")
                                .FontSize(13).FontColor("#718096");
                        });
                        row.ConstantItem(120).Column(col =>
                        {
                            col.Item().AlignRight().Text(report.PatientName).Bold();
                            col.Item().AlignRight().Text(
                                $"{report.PeriodStart} → {report.PeriodEnd}")
                                .FontSize(10).FontColor("#718096");
                        });
                    });
                });

                // Content
                page.Content().PaddingTop(10).Column(col =>
                {
                    // Adherence Summary
                    col.Item().Background("#f0fff4").Padding(10).Column(s =>
                    {
                        s.Item().Text("📊 ملخص الالتزام").Bold().FontSize(13);
                        s.Item().Text(
                            $"نسبة الالتزام: {report.AdherenceStats.AdherencePercentage}%");
                        s.Item().Text(
                            $"الجرعات المأخوذة: {report.AdherenceStats.TakenDoses}/{report.AdherenceStats.TotalDoses}");
                        s.Item().Text(
                            $"الجرعات الفائتة: {report.AdherenceStats.MissedDoses}");
                        s.Item().Text(
                            $"السلسلة الحالية: {report.AdherenceStats.CurrentStreak} أيام");
                    });

                    col.Item().PaddingTop(12);

                    // Medications Table
                    col.Item().Text("💊 الأدوية الحالية").Bold().FontSize(13);
                    col.Item().PaddingTop(4).Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn(3);
                            cols.RelativeColumn(2);
                            cols.RelativeColumn(2);
                            cols.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            foreach (var h in new[] {
                                "اسم الدواء", "الجرعة", "عدد المرات", "وقت الأخذ" })
                            {
                                header.Cell().Background("#1a56a0")
                                    .Padding(5).Text(h).FontColor("#ffffff").Bold();
                            }
                        });

                        foreach (var med in report.Medications)
                        {
                            table.Cell().Padding(5).Text(med.BrandName);
                            table.Cell().Padding(5).Text(
                                $"{med.DosageValue} {med.DosageUnit}");
                            table.Cell().Padding(5).Text(
                                $"{med.TimesPerDay} مرة/يوم");
                            table.Cell().Padding(5).Text(med.ScheduleType switch {
                                "with_food"    => "مع الأكل",
                                "after_food"   => "بعد الأكل",
                                "empty_stomach" => "على الريق",
                                "before_sleep"  => "قبل النوم",
                                _ => med.ScheduleType
                            });
                        }
                    });

                    // Interactions Warning
                    if (report.Interactions.Any())
                    {
                        col.Item().PaddingTop(12);
                        col.Item().Text("⚠️ تحذيرات التفاعلات الدوائية")
                            .Bold().FontSize(13).FontColor("#c53030");
                        foreach (var inter in report.Interactions)
                        {
                            col.Item().Background("#fff5f5").Padding(8).Column(w =>
                            {
                                w.Item().Text(
                                    $"{inter.Drug1Name} + {inter.Drug2Name}").Bold();
                                w.Item().Text(inter.DescriptionAr).FontSize(10);
                            });
                        }
                    }

                    // Health Metrics
                    if (report.HealthMetrics.Any())
                    {
                        col.Item().PaddingTop(12);
                        col.Item().Text("❤️ القياسات الصحية").Bold().FontSize(13);
                        foreach (var h in report.HealthMetrics.Take(10))
                        {
                            col.Item().Text(
                                $"• {h.MetricType}: {h.Value} {h.Unit} — {h.RecordedAt[..10]}")
                                .FontSize(10);
                        }
                    }
                });

                // Footer
                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("MediSync — صُنع بمحبة | الصفحة ").FontSize(9);
                    text.CurrentPageNumber().FontSize(9);
                    text.Span(" من ").FontSize(9);
                    text.TotalPages().FontSize(9);
                });
            });
        }).GeneratePdf();
    }
}