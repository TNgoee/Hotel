public class ReviewRoomEditDto
{
    public required string CustomId { get; set; }
    public required string RoomId { get; set; }
    public int? Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}