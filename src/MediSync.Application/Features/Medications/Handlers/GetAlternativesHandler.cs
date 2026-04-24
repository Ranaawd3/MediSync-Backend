using MediatR;
using MediSync.Application.Features.Medications.Queries;
using MediSync.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediSync.Application.Features.Medications.Handlers;

public class GetAlternativesHandler(IApplicationDbContext db)
    : IRequestHandler<GetAlternativesQuery, List<string>>
{
    public async Task<List<string>> Handle(
        GetAlternativesQuery req, CancellationToken ct)
    {
        var med = await db.Medications
            .FirstOrDefaultAsync(m => m.Id == req.MedicationId
                                   && m.UserId == req.UserId, ct)
            ?? throw new KeyNotFoundException("MEDICATION_NOT_FOUND");

        // جيب البدائل من DrugInteractions table
        var interactions = await db.DrugInteractions
            .Where(d => d.Drug1Ingredient == med.ActiveIngredient.ToLower()
                     || d.Drug2Ingredient == med.ActiveIngredient.ToLower())
            .SelectMany(d => d.Alternatives)
            .Distinct()
            .ToListAsync(ct);

        return interactions;
    }
}