namespace MediSync.Domain.Entities;

public class Medication
{
    public Guid     Id               { get; set; } = Guid.NewGuid();
    public Guid     UserId           { get; set; }
    public string   BrandName        { get; set; } = "";
    public string   GenericName      { get; set; } = "";
    public string   ActiveIngredient { get; set; } = "";
    public decimal  DosageValue      { get; set; }
    public string   DosageUnit       { get; set; } = "mg";
    public string   Form             { get; set; } = "tablet";
    public int      TimesPerDay      { get; set; }
    public string   ScheduleType     { get; set; } = "with_food";
    public int?     DurationDays     { get; set; }
    public DateOnly StartDate        { get; set; }
    public DateOnly? EndDate         { get; set; }
    public int?     StockCount       { get; set; }
    public string   ColorCode        { get; set; } = "blue";
    public string   Source           { get; set; } = "manual";
    public bool     IsActive         { get; set; } = true;
    public DateTime CreatedAt        { get; set; } = DateTime.UtcNow;

    // Navigation
    public User     User      { get; set; } = null!;
    public ICollection<Reminder> Reminders { get; set; } = [];
}