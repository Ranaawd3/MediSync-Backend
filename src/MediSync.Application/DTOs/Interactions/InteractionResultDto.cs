namespace MediSync.Application.DTOs.Interactions;

// متوافق مع Contract Section 7 — Interaction Variables
public record InteractionResultDto(
    string       Drug1Name,
    string       Drug2Name,
    string       Drug1Ingredient,
    string       Drug2Ingredient,
    string       Severity,          // HIGH | MODERATE | LOW
    string       DescriptionAr,
    string       DescriptionEn,
    string?      Mechanism,
    List<string> Alternatives,
    List<string> FoodWarnings
);
