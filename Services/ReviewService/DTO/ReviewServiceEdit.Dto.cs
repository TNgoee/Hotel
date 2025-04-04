public class ReviewServiceEditDto
{
    public required string CustomId { get; set; }
    public required string ServiceId { get; set; }
    public int? Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}