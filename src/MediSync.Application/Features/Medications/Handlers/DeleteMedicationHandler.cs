using MediatR;
using MediSync.Application.Features.Medications.Commands;
using MediSync.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediSync.Application.Features.Medications.Handlers;

public class DeleteMedicationHandler(IApplicationDbContext db)
    : IRequestHandler<DeleteMedicationCommand>
{
    public async Task Handle(DeleteMedicationCommand req, CancellationToken ct)
    {
        var m = await db.Medications
            .FirstOrDefaultAsync(x => x.Id == req.MedicationId
                                   && x.UserId == req.UserId, ct)
            ?? throw new KeyNotFoundException("MEDICATION_NOT_FOUND");

        m.IsActive = false; // Soft Delete
        await db.SaveChangesAsync(ct);
    }
}