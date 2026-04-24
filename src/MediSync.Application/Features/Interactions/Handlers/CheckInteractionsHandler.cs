using MediatR;
using MediSync.Application.DTOs.Interactions;
using MediSync.Application.Features.Interactions.Queries;
using MediSync.Application.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace MediSync.Application.Features.Interactions.Handlers;

public class CheckInteractionsHandler(IApplicationDbContext db, IDistributedCache cache)
    : IRequestHandler<CheckInteractionsQuery, List<InteractionResultDto>>
{
    public async Task<List<InteractionResultDto>> Handle(
        CheckInteractionsQuery req, CancellationToken ct)
    {
        // 1. جيب كل أدوية المستخدم الفعّالة
        var medications = await db.Medications
            .Where(m => m.UserId == req.UserId && m.IsActive)
            .Select(m => new { m.BrandName, Ingredient = m.ActiveIngredient.ToLower() })
            .ToListAsync(ct);

        if (medications.Count < 2)
            return [];

        var results = new List<InteractionResultDto>();

        // 2. Pairwise check لكل جوزين أدوية
        for (int i = 0; i < medications.Count; i++)
        for (int j = i + 1; j < medications.Count; j++)
        {
            var ing1 = medications[i].Ingredient;
            var ing2 = medications[j].Ingredient;
            var cacheKey = $"interaction:{ing1}:{ing2}";

            // 3. شوف في Redis Cache الأول
            InteractionResultDto? result = null;
            var cached = await cache.GetStringAsync(cacheKey, ct);
            if (cached != null)
            {
                result = JsonSerializer.Deserialize<InteractionResultDto>(cached);
            }
            else
            {
                // 4. Hybrid Strategy: DB محلية الأول
                var dbInteraction = await db.DrugInteractions
                    .FirstOrDefaultAsync(d =>
                        (d.Drug1Ingredient == ing1 && d.Drug2Ingredient == ing2) ||
                        (d.Drug1Ingredient == ing2 && d.Drug2Ingredient == ing1), ct);

                if (dbInteraction != null)
                {
                    result = new InteractionResultDto(
                        medications[i].BrandName, medications[j].BrandName,
                        ing1, ing2,
                        dbInteraction.Severity,
                        dbInteraction.DescriptionAr,
                        dbInteraction.DescriptionEn,
                        dbInteraction.Mechanism,
                        dbInteraction.Alternatives,
                        []
                    );
                }
                // 5. لو مش موجود في DB — هنا بيجي DrugBank API (أسبوع لاحق)
                // result = await _drugBankService.CheckAsync(ing1, ing2, ct);

                // 6. حفظ في Redis Cache (TTL 24 ساعة)
                if (result != null)
                {
                    var json = JsonSerializer.Serialize(result);
                    await cache.SetStringAsync(cacheKey, json, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
                    }, ct);
                }
            }

            if (result != null)
                results.Add(result);
        }

        return results;
    }
}