public class CustomerDiscountEditDto
{
    public required string CustomerId { get; set; }
    public required string DiscountId { get; set; }
    public required bool IsUsed { get; set; }
    public required DateTime ReceivedAt { get; set; }
    public DateTime? UsedAt { get; set; } = null;
}