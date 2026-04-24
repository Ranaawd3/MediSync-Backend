using MediatR;
using MediSync.Application.DTOs.Common;
using MediSync.Application.DTOs.Medications;

namespace MediSync.Application.Features.Medications.Commands;

public record AddMedicationCommand(AddMedicationDto Dto, Guid UserId)
    : IRequest<AddMedicationResult>;

public record AddMedicationResult(
    MedicationDto           Medication,
    bool                    HasWarnings,
    List<WarningDto>         Warnings
);