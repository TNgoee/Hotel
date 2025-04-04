public class DiscountEditDto
{
    public string? NameDiscount { get; set; }
    public required decimal Price { get; set; }
    public required string Description { get; set; }
    public required string Img { get; set; }
    public decimal? MinAmount { get; set; }
    public int? MaxQuantity { get; set; }
    public int UsedQuantity { get; set; } = 0;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

}