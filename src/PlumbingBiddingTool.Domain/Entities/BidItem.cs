namespace PlumbingBiddingTool.Domain.Entities;

public class BidItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
