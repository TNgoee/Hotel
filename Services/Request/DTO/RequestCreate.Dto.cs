public class RequestCreateDto
{
    public required string CustomerId { get; set; }
    public required string RoomTypeId { get; set; }
    public string? AssignedRoomId { get; set; }
    public required DateTime DateStart { get; set; }
    public required DateTime DateEnd { get; set; }
    public required RequestStatus Status { get; set; }
    public required decimal TotalAmount { get; set; }
    public string? DiscountId { get; set; }
    public required decimal FinalAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
public enum RequestStatus
{
    Pending,
    Confirmed,
    Canceled,
    Completed
}