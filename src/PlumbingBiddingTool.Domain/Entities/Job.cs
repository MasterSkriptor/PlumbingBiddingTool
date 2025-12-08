namespace PlumbingBiddingTool.Domain.Entities;

public class Job
{
    public int Id { get; set; }
    public string JobName { get; set; } = string.Empty;
    public JobStatus Status { get; set; } = JobStatus.Open;
    
    public int ContractorId { get; set; }
    public Contractor Contractor { get; set; } = null!;
    
    public ICollection<JobFixtureItem> JobFixtureItems { get; set; } = new List<JobFixtureItem>();
    
    public ICollection<JobOption> JobOptions { get; set; } = new List<JobOption>();
    
    public decimal TotalCost => (JobFixtureItems?.Sum(jfi => jfi.Price * jfi.Quantity) ?? 0) + 
                                (JobOptions?.Sum(jo => jo.Price * jo.Quantity) ?? 0);
}
