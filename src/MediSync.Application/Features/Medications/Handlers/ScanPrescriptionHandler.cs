using MediatR;
using System.Text.Json;
using MediSync.Application.DTOs.Medications;
using MediSync.Application.Features.Medications.Commands;
using MediSync.Application.Persistence;
using MediSync.Application.Services;
using MediSync.Domain.Entities;

namespace MediSync.Application.Features.Medications.Handlers;

public class ScanPrescriptionHandler(
    IApplicationDbContext db,
    IOcrService           ocrService)
    : IRequestHandler<ScanPrescriptionCommand, OcrResultDto>
{
    public async Task<OcrResultDto> Handle(
        ScanPrescriptionCommand req, CancellationToken ct)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();

        OcrResultDto result;
        try
        {
            result = await ocrService.ScanAsync(
                req.ImageBytes,
                req.FileName,
                req.ContentType,
                ct);
        }
        catch
        {
            result = new OcrResultDto(
                Guid.NewGuid(), 0, "failed", true, null, [], 0);
        }

        sw.Stop();

        var scan = new OcrScan
        {
            UserId        = req.UserId,
            RawText       = result.RawText,
            ExtractedData = JsonSerializer.Serialize(result.ExtractedMedications),
            Confidence    = result.Confidence,
            Status        = result.Status,
        };
        db.OcrScans.Add(scan);
        await db.SaveChangesAsync(ct);

        return result with
        {
            ScanId               = scan.Id,
            RequiresManualReview = result.Confidence < 70,
            ProcessingTimeMs     = (int)sw.ElapsedMilliseconds
        };
    }
}