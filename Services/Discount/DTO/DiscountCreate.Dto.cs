public class DiscountCreateDto
{
    public required string NameDiscount { get; set; }
    public required decimal Price { get; set; }
    public required string Description { get; set; }
    public required decimal MinAmount { get; set; }
    public required int MaxQuantity { get; set; }
    public int UsedQuantity { get; set; } = 0;
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }

}