namespace MediSync.Domain.Entities;

public class DrugInteraction
{
    public Guid          Id               { get; set; } = Guid.NewGuid();
    public string        Drug1Ingredient  { get; set; } = "";
    public string        Drug2Ingredient  { get; set; } = "";
    public string        Severity         { get; set; } = "LOW";
    public string        DescriptionAr    { get; set; } = "";
    public string        DescriptionEn    { get; set; } = "";
    public string?       Mechanism        { get; set; }
    public List<string>  Alternatives     { get; set; } = [];
    public string        Source           { get; set; } = "DrugBank";
    public DateOnly      LastReviewed     { get; set; }
}