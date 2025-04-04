public class RoomTypeCreateDto
{
    public required string RoomTypeName { get; set; }
    public required string Description { get; set; }
    public required decimal Price { get; set; }
    public int RoomCount { get; set; } = 0;
    public required List<string> ServiceIds { get; set; } = new();
}