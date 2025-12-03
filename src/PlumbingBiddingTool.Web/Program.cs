using PlumbingBiddingTool.Web.Components;
using Microsoft.EntityFrameworkCore;
using PlumbingBiddingTool.Application.BidItems;
using PlumbingBiddingTool.Application.FixtureItems;
using PlumbingBiddingTool.Application.Contractors;
using PlumbingBiddingTool.Application.Jobs;
using PlumbingBiddingTool.Domain.Repositories;
using PlumbingBiddingTool.Infrastructure.Data;
using PlumbingBiddingTool.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Data Source=plumbingbidding.db"));

// Register repositories
builder.Services.AddScoped<IBidItemRepository, BidItemRepository>();
builder.Services.AddScoped<IFixtureItemRepository, FixtureItemRepository>();
builder.Services.AddScoped<IContractorRepository, ContractorRepository>();
builder.Services.AddScoped<IJobRepository, JobRepository>();

// Register application services
builder.Services.AddScoped<BidItemService>();
builder.Services.AddScoped<FixtureItemService>();
builder.Services.AddScoped<ContractorService>();
builder.Services.AddScoped<JobService>();

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
