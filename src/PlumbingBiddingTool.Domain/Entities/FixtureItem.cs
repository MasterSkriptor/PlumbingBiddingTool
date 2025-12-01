namespace PlumbingBiddingTool.Domain.Entities;

public class FixtureItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<BidItem> BidItems { get; set; } = new List<BidItem>();
    
    public decimal Price => BidItems?.Sum(bi => bi.Price) ?? 0;
}
