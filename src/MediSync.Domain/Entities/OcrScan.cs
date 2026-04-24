namespace MediSync.Domain.Entities;

public class OcrScan
{
    public Guid     Id             { get; set; } = Guid.NewGuid();
    public Guid     UserId         { get; set; }
    public string?  ImageUrl       { get; set; }
    public string?  RawText        { get; set; }
    public string?  ExtractedData  { get; set; } // JSON string
    public decimal  Confidence     { get; set; }
    public string   Status         { get; set; } = "success";
    public DateTime CreatedAt      { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}