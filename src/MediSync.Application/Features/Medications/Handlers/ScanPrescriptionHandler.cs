using MediatR;
using MediSync.Application.DTOs.Medications;
using MediSync.Application.Features.Medications.Commands;
using MediSync.Domain.Entities;
using MediSync.Application.Persistence;

namespace MediSync.Application.Features.Medications.Handlers;

public class ScanPrescriptionHandler(IApplicationDbContext db)
    : IRequestHandler<ScanPrescriptionCommand, OcrResultDto>
{
    public async Task<OcrResultDto> Handle(
        ScanPrescriptionCommand req, CancellationToken ct)
    {
        var scan = new OcrScan
        {
            UserId     = req.UserId,
            Status     = "pending",
            Confidence = 0
        };
        db.OcrScans.Add(scan);
        await db.SaveChangesAsync(ct);

        return new OcrResultDto(scan.Id, 0, "pending", true, null, [], 0);
    }
}