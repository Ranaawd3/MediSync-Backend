namespace MediSync.Domain.Entities;

public class DrugNameMapping
{
    public Guid   Id              { get; set; } = Guid.NewGuid();
    public string LocalName       { get; set; } = ""; // "بروفين"
    public string GenericName     { get; set; } = ""; // "Ibuprofen"
    public string ActiveIngredient { get; set; } = ""; // "ibuprofen"
    public string Country         { get; set; } = "EG";
    public string Source          { get; set; } = "manual";
}