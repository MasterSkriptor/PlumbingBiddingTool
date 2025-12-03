namespace PlumbingBiddingTool.Domain.Entities;

public class Contractor
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Job> Jobs { get; set; } = new List<Job>();
}
