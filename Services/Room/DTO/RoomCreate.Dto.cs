using KhachSan.Models;

public class RoomCreateDto
{
    public required string NameRoom { get; set; }
    public required int Floor { get; set; }
    public required RoomStatus Status { get; set; }
    public required string RoomTypeId { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}