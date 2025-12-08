namespace PlumbingBiddingTool.Domain.Entities;

public class JobOption
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    
    // Navigation
    public Job? Job { get; set; }
}
