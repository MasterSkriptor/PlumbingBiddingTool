namespace PlumbingBiddingTool.Domain.Entities;

public class JobFixtureItem
{
    public int Id { get; set; }
    
    public int JobId { get; set; }
    public Job Job { get; set; } = null!;
    
    public int FixtureItemId { get; set; }
    public FixtureItem FixtureItem { get; set; } = null!;
    
    public int Quantity { get; set; }
    public decimal Price { get; set; } // Snapshot of FixtureItem price at time of creation
}
