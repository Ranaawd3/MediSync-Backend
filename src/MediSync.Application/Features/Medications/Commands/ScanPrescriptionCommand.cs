using MediatR;
using MediSync.Application.DTOs.Medications;

namespace MediSync.Application.Features.Medications.Commands;

public record ScanPrescriptionCommand(
    byte[]  ImageBytes,    // ✅ مش IFormFile — Application Layer مش بيعرف IFormFile
    string  FileName,
    string  ContentType,
    Guid    UserId
) : IRequest<OcrResultDto>;