using System.IO;
using Microsoft.Data.Sqlite;
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

// Configure Database: place SQLite DB in the Infrastructure project folder
var dbPath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "PlumbingBiddingTool.Infrastructure", "plumbingbidding.db"));
var baseConnection = builder.Configuration.GetConnectionString("DefaultConnection") ?? $"Data Source={dbPath}";
var sqliteBuilder = new SqliteConnectionStringBuilder(baseConnection);
if (!Path.IsPathRooted(sqliteBuilder.DataSource))
{
    sqliteBuilder.DataSource = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, sqliteBuilder.DataSource));
}
var connectionString = sqliteBuilder.ToString();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Register repositories
builder.Services.AddScoped<IBidItemRepository, BidItemRepository>();
builder.Services.AddScoped<IFixtureItemRepository, FixtureItemRepository>();
builder.Services.AddScoped<IContractorRepository, ContractorRepository>();
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<IJobOptionRepository, JobOptionRepository>();

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
