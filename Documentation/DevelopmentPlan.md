# Development Plan - Plumbing Bidding Tool

**Project:** Plumbing Bidding Tool  
**Version:** 1.0  
**Team:** One Developer + GitHub Copilot  
**Current Status:** MVP Foundation Complete  
**Last Updated:** February 2025

---

## Table of Contents

1. [Overview](#overview)
2. [Current State Assessment](#current-state-assessment)
3. [Development Environment](#development-environment)
4. [Enhancement Roadmap](#enhancement-roadmap)
5. [Implementation Guidelines](#implementation-guidelines)
6. [Testing Strategy](#testing-strategy)
7. [Deployment Strategy](#deployment-strategy)
8. [Maintenance & Support](#maintenance--support)

---

## Overview

This development plan outlines the ongoing development approach for the Plumbing Bidding Tool using a one developer + GitHub Copilot team. The application follows Clean Architecture principles and is built with Blazor Server on .NET 8.

### Development Philosophy
- **Clean Architecture:** Maintain strict separation of concerns across layers
- **Incremental Delivery:** Add features in small, tested increments
- **User-Centered:** Prioritize features that save time and reduce errors
- **Quality First:** Automated testing and code review before deployment
- **Copilot Integration:** Leverage AI assistance for boilerplate, tests, and patterns

---

## Current State Assessment

### What's Working (MVP Foundation ✅)

#### Domain Layer
- ✅ Core entities: Job, Contractor, FixtureItem, BidItem, JobOption, JobFixtureItem
- ✅ Enumerations: Phase, ItemType, JobStatus
- ✅ Relationships properly modeled
- ✅ Value calculations in domain (TotalCost, fixture prices)

#### Application Layer
- ✅ Service classes for all main entities
- ✅ JobService with complete CRUD operations
- ✅ ContractorService for contractor management
- ✅ BidItemService for item management
- ✅ FixtureItemService for fixture management
- ✅ Repository interfaces defined

#### Infrastructure Layer
- ✅ Entity Framework Core with SQLite
- ✅ ApplicationDbContext configured
- ✅ Repository implementations for all entities
- ✅ Entity configurations for relationships
- ✅ Database migrations working
- ✅ Connection string management

#### Web Layer (Blazor)
- ✅ Interactive Server rendering mode
- ✅ Main layout with navigation
- ✅ Job pages (Index, Create, Edit, Details)
- ✅ Contractor pages (Index)
- ✅ BidItems pages (Index)
- ✅ FixtureItems pages (Index)
- ✅ Real-time calculation updates
- ✅ Form validation

### Known Gaps & Improvement Areas

#### Features
- ⚠️ No PDF/Excel export for bids
- ⚠️ Limited search and filtering
- ⚠️ No job cloning capability
- ⚠️ Settings page incomplete
- ⚠️ No user authentication
- ⚠️ No data backup/export tools

#### Technical Debt
- ⚠️ Limited unit test coverage
- ⚠️ No integration tests
- ⚠️ Error handling could be more comprehensive
- ⚠️ No logging framework implemented
- ⚠️ SQLite not ideal for production multi-user scenarios

#### User Experience
- ⚠️ Could benefit from better loading indicators
- ⚠️ No keyboard shortcuts
- ⚠️ Mobile experience could be optimized
- ⚠️ No inline help or tooltips

---

## Development Environment

### Prerequisites

1. **Development Tools**
   - Visual Studio 2022 or VS Code with C# Dev Kit
   - .NET 8 SDK
   - Git
   - SQL Server Management Studio (optional, for future migration)

2. **Installation & Setup**
   ```bash
   # Clone the repository
   git clone https://github.com/MasterSkriptor/PlumbingBiddingTool.git
   cd PlumbingBiddingTool
   
   # Restore dependencies
   dotnet restore
   
   # Build solution
   dotnet build
   
   # Run database migrations
   cd src/PlumbingBiddingTool.Web
   dotnet ef database update
   
   # Run the application
   dotnet run
   ```

3. **Database Location**
   - SQLite database: `src/PlumbingBiddingTool.Infrastructure/plumbingbidding.db`
   - Configured in `Program.cs` with fallback to Infrastructure folder

4. **Project Structure**
   ```
   PlumbingBiddingTool/
   ├── src/
   │   ├── PlumbingBiddingTool.Domain/
   │   │   ├── Entities/
   │   │   └── Repositories/ (interfaces)
   │   ├── PlumbingBiddingTool.Application/
   │   │   ├── BidItems/
   │   │   ├── FixtureItems/
   │   │   ├── Contractors/
   │   │   └── Jobs/
   │   ├── PlumbingBiddingTool.Infrastructure/
   │   │   ├── Data/
   │   │   ├── Repositories/
   │   │   ├── Config/
   │   │   └── Migrations/
   │   └── PlumbingBiddingTool.Web/
   │       ├── Components/
   │       │   ├── Layout/
   │       │   └── Pages/
   │       └── wwwroot/
   └── tests/
       ├── PlumbingBiddingTool.Domain.Tests/
       ├── PlumbingBiddingTool.Application.Tests/
       └── PlumbingBiddingTool.Infrastructure.Tests/
   ```

---

## Enhancement Roadmap

### Phase 1: Core Improvements (Weeks 1-4)

#### Week 1: Testing Foundation
**Goal:** Establish comprehensive test coverage for critical paths

- [ ] **Task 1.1:** Set up xUnit test projects (already exist, verify configuration)
- [ ] **Task 1.2:** Add unit tests for Domain entities
  ```
  Tests for:
  - Job.TotalCost calculation
  - FixtureItem.Price calculation
  - Phase and ItemType enumerations
  - JobStatus state transitions
  ```
  - **Copilot Prompt:** "Generate xUnit tests for Job entity TotalCost calculation with fixtures and options"

- [ ] **Task 1.3:** Add unit tests for Application services
  ```
  Tests for:
  - JobService.CreateJobAsync
  - JobService.UpdateJobAsync
  - BidItemService CRUD operations
  - ContractorService operations
  ```
  - **Copilot Prompt:** "Generate xUnit tests for JobService with mocked repositories"

- [ ] **Task 1.4:** Add integration tests for database operations
  ```
  Tests for:
  - Repository CRUD operations
  - Entity Framework relationships
  - Migration scenarios
  ```

- [ ] **Task 1.5:** Set up test data seeding helpers
  - Create test fixture builders
  - Mock data generators

**Deliverables:** 60%+ code coverage on Domain and Application layers

#### Week 2: User Experience Enhancements
**Goal:** Improve usability and add frequently requested features

- [ ] **Task 2.1:** Implement job cloning
  ```
  In JobService:
  - CloneJobAsync(int jobId, string newJobName)
  - Copy all fixtures and options
  - Reset status to Open
  ```
  - **Copilot Prompt:** "Implement job cloning method that duplicates all fixtures and options"

- [ ] **Task 2.2:** Add search and filtering to Jobs page
  ```
  - Filter by contractor
  - Filter by status
  - Search by job name
  - Sort by date, cost, status
  ```

- [ ] **Task 2.3:** Improve loading states
  ```
  - Add loading indicators to all data operations
  - Implement skeleton screens for lists
  - Add progress feedback for long operations
  ```

- [ ] **Task 2.4:** Add confirmation dialogs
  ```
  - Delete confirmations for jobs, contractors, items
  - Unsaved changes warnings
  - Success/error toast notifications
  ```

- [ ] **Task 2.5:** Implement keyboard shortcuts
  ```
  - Ctrl+N: New job
  - Ctrl+S: Save
  - Ctrl+F: Search
  - Esc: Cancel/close
  ```

**Deliverables:** Enhanced UX with search, cloning, and better feedback

#### Week 3: Reporting & Export
**Goal:** Enable users to export and analyze bid data

- [ ] **Task 3.1:** Install PDF generation library
  ```bash
  dotnet add package QuestPDF
  ```
  - Evaluate QuestPDF for clean, code-based PDF generation

- [ ] **Task 3.2:** Create BidPdfGenerator service
  ```
  In PlumbingBiddingTool.Application.Jobs:
  - IBidPdfGenerator interface
  - BidPdfGenerator implementation
  - Generate professional bid documents
  - Include contractor info, fixtures, options, totals
  - Add company branding (logo, colors)
  ```
  - **Copilot Prompt:** "Create PDF generator service for job bids with QuestPDF including fixtures table and totals"

- [ ] **Task 3.3:** Add PDF download to Job Details page
  ```
  - Add "Download PDF" button
  - Generate PDF on-demand
  - Return as file download
  ```

- [ ] **Task 3.4:** Create Excel export for job lists
  ```
  - Install ClosedXML or EPPlus
  - Export jobs to Excel with all details
  - Include summary sheet with totals
  ```

- [ ] **Task 3.5:** Add cost breakdown reports
  ```
  - Report by phase (Underground, Stack Out, Trim)
  - Report by item type (Sewer, Water, Gas)
  - Visual charts if possible (Chart.js integration)
  ```

**Deliverables:** PDF bid export and Excel reporting

#### Week 4: Data Management & Settings
**Goal:** Improve data integrity and configurability

- [ ] **Task 4.1:** Implement Settings page
  ```
  Settings to include:
  - Company information (name, logo, contact)
  - Default pricing markup %
  - Tax rate configuration
  - Bid template defaults
  ```

- [ ] **Task 4.2:** Add bulk import for bid items
  ```
  - CSV upload for bid items
  - Validation and error reporting
  - Preview before import
  ```

- [ ] **Task 4.3:** Implement data validation
  ```
  - Price validation (must be > 0)
  - Quantity validation (must be > 0)
  - Name uniqueness checks
  - Required field validation
  ```

- [ ] **Task 4.4:** Add data backup/export
  ```
  - Export entire database to JSON
  - Import from backup
  - Scheduled backups (if deployed)
  ```

- [ ] **Task 4.5:** Implement soft delete
  ```
  - Add IsDeleted flag to entities
  - Filter out deleted items from queries
  - Add "restore" functionality
  - Permanent delete after X days
  ```

**Deliverables:** Settings management and data tools

---

### Phase 2: Advanced Features (Weeks 5-8)

#### Week 5: Database Migration & Multi-User Prep
**Goal:** Prepare for production deployment

- [ ] **Task 5.1:** Migrate to SQL Server or PostgreSQL
  ```
  - Install appropriate EF provider
  - Update connection string configuration
  - Test all migrations
  - Verify performance
  ```
  - **Copilot Prompt:** "Update ApplicationDbContext to support SQL Server with connection string from configuration"

- [ ] **Task 5.2:** Implement database seeding
  ```
  - Create initial fixture library
  - Seed common bid items
  - Sample contractors for demo
  ```

- [ ] **Task 5.3:** Add logging framework
  ```
  - Install Serilog
  - Configure file and console logging
  - Add structured logging to all services
  - Log errors, warnings, and important events
  ```

- [ ] **Task 5.4:** Implement error handling middleware
  ```
  - Global exception handler
  - User-friendly error pages
  - Error logging and monitoring
  ```

#### Week 6: Authentication & Authorization
**Goal:** Add user management and security

- [ ] **Task 6.1:** Implement ASP.NET Core Identity
  ```
  - Install Identity packages
  - Add ApplicationUser entity
  - Configure authentication in Program.cs
  - Add login/logout pages
  ```
  - **Copilot Prompt:** "Implement ASP.NET Core Identity with Blazor Server including login and registration pages"

- [ ] **Task 6.2:** Add role-based authorization
  ```
  Roles:
  - Admin: Full access
  - Estimator: Create/edit jobs and items
  - Viewer: Read-only access
  ```

- [ ] **Task 6.3:** Update entities with user tracking
  ```
  - Add CreatedBy, ModifiedBy fields
  - Track creation and modification dates
  - Audit trail for important changes
  ```

- [ ] **Task 6.4:** Implement multi-tenancy prep
  ```
  - Add CompanyId to entities if needed
  - Filter queries by user's company
  - Ensure data isolation
  ```

#### Week 7: Analytics & Reporting
**Goal:** Provide business insights

- [ ] **Task 7.1:** Create analytics dashboard
  ```
  Metrics:
  - Total bids this month
  - Average bid amount
  - Bids by status
  - Top contractors by volume
  - Cost breakdown by phase
  ```

- [ ] **Task 7.2:** Implement bid history tracking
  ```
  - Track bid versions
  - Show changes over time
  - Compare original vs current pricing
  ```

- [ ] **Task 7.3:** Add won/lost tracking
  ```
  - Add WonLostStatus to Job
  - Track reasons (price, timeline, other)
  - Calculate win rate
  - Analyze lost bid patterns
  ```

- [ ] **Task 7.4:** Create pricing analysis reports
  ```
  - Historical pricing trends
  - Most/least profitable jobs
  - Fixture usage frequency
  - Price variance analysis
  ```

#### Week 8: Polish & Performance
**Goal:** Optimize and refine

- [ ] **Task 8.1:** Performance optimization
  ```
  - Add database indexes
  - Optimize EF queries (Include vs Select)
  - Implement query result caching
  - Pagination for large lists
  ```

- [ ] **Task 8.2:** UI/UX improvements
  ```
  - Consistent styling across all pages
  - Improved mobile responsive design
  - Accessibility improvements (ARIA labels, keyboard nav)
  - Add help tooltips and inline guidance
  ```

- [ ] **Task 8.3:** Code cleanup
  ```
  - Remove commented code
  - Consolidate duplicate logic
  - Improve naming consistency
  - Add XML documentation
  ```

- [ ] **Task 8.4:** Security hardening
  ```
  - SQL injection protection (EF handles)
  - XSS prevention
  - CSRF protection (built-in Blazor)
  - Input sanitization
  - Secure headers configuration
  ```

---

### Phase 3: Integration & Scale (Weeks 9-12)

#### Week 9-10: QuickBooks Integration (Optional)
**Goal:** Automate invoicing and accounting

- [ ] **Task 9.1:** QuickBooks OAuth setup
  ```
  - Register app in Intuit Developer Portal
  - Implement OAuth 2.0 flow
  - Store and refresh tokens
  ```

- [ ] **Task 9.2:** Customer sync
  ```
  - Map Contractors to QB Customers
  - Sync contact information
  - Handle updates bidirectionally
  ```

- [ ] **Task 9.3:** Invoice generation
  ```
  - Create QB invoice from Job
  - Map line items appropriately
  - Send invoice automatically
  ```

#### Week 11: API Development
**Goal:** Enable third-party integrations

- [ ] **Task 11.1:** Design REST API
  ```
  Endpoints:
  - GET/POST/PUT/DELETE for all entities
  - Bulk operations
  - Search and filtering
  - Pagination support
  ```

- [ ] **Task 11.2:** Implement API controllers
  ```
  - Separate API project or controllers
  - DTO mapping (AutoMapper)
  - API versioning
  - Rate limiting
  ```

- [ ] **Task 11.3:** Add API documentation
  ```
  - Swagger/OpenAPI
  - Example requests/responses
  - Authentication documentation
  ```

#### Week 12: Deployment & DevOps
**Goal:** Production-ready deployment

- [ ] **Task 12.1:** Containerize application
  ```
  - Create Dockerfile
  - Docker Compose for development
  - Optimize image size
  ```

- [ ] **Task 12.2:** Set up CI/CD
  ```
  - GitHub Actions or Azure DevOps
  - Automated build and test
  - Automated deployment to staging
  - Manual approval for production
  ```

- [ ] **Task 12.3:** Cloud deployment
  ```
  Options:
  - Azure App Service
  - AWS Elastic Beanstalk
  - Digital Ocean
  - Self-hosted VPS
  ```

- [ ] **Task 12.4:** Monitoring and observability
  ```
  - Application Insights or similar
  - Health checks
  - Uptime monitoring
  - Performance metrics
  - Error tracking
  ```

---

## Implementation Guidelines

### Clean Architecture Best Practices

#### Dependency Rules
1. **Domain** has NO dependencies (pure C#)
   - Entities
   - Value Objects
   - Repository Interfaces
   - Domain Services (if needed)

2. **Application** depends only on Domain
   - Use Cases / Services
   - DTOs
   - Application Interfaces

3. **Infrastructure** depends on Application and Domain
   - Repository Implementations
   - Database Context
   - External Service Implementations

4. **Web** depends on all layers (Composition Root)
   - Blazor Components
   - Dependency Injection Configuration
   - Startup/Program Configuration

### Coding Standards

#### General
- Use C# 12 language features where appropriate
- Follow Microsoft naming conventions
- Async all the way (no sync-over-async)
- Use nullable reference types
- Prefer records for DTOs

#### Services
```csharp
// Example service pattern
public class JobService
{
    private readonly IJobRepository _repository;
    private readonly ILogger<JobService> _logger;
    
    public JobService(IJobRepository repository, ILogger<JobService> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public async Task<Job> CreateJobAsync(...)
    {
        _logger.LogInformation("Creating job {JobName}", jobName);
        
        // Validation
        // Business logic
        // Save
        
        return job;
    }
}
```

#### Blazor Components
```csharp
// Example component pattern
@page "/jobs/create"
@inject JobService JobService
@inject NavigationManager Navigation
@rendermode InteractiveServer

@code {
    private JobModel model = new();
    private bool isLoading = false;
    
    private async Task HandleSubmit()
    {
        isLoading = true;
        try
        {
            await JobService.CreateJobAsync(...);
            Navigation.NavigateTo("/jobs");
        }
        catch (Exception ex)
        {
            // Error handling
        }
        finally
        {
            isLoading = false;
        }
    }
}
```

### Database Patterns

#### Repository Pattern
```csharp
public interface IJobRepository
{
    Task<Job?> GetByIdAsync(int id);
    Task<IEnumerable<Job>> GetAllAsync();
    Task<Job> AddAsync(Job job);
    Task UpdateAsync(Job job);
    Task DeleteAsync(int id);
}
```

#### Entity Configuration
```csharp
public class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.HasKey(j => j.Id);
        builder.Property(j => j.JobName).IsRequired().HasMaxLength(200);
        builder.HasOne(j => j.Contractor)
               .WithMany(c => c.Jobs)
               .HasForeignKey(j => j.ContractorId);
    }
}
```

---

## Testing Strategy

### Unit Testing (Target: 70% Coverage)

#### Domain Tests
```csharp
public class JobTests
{
    [Fact]
    public void TotalCost_CalculatesCorrectly()
    {
        // Arrange
        var job = new Job
        {
            JobFixtureItems = new List<JobFixtureItem>
            {
                new() { Price = 100, Quantity = 2 }  // 200
            },
            JobOptions = new List<JobOption>
            {
                new() { Price = 50, Quantity = 1 }   // 50
            }
        };
        
        // Act
        var total = job.TotalCost;
        
        // Assert
        Assert.Equal(250, total);
    }
}
```

#### Application Tests
```csharp
public class JobServiceTests
{
    [Fact]
    public async Task CreateJobAsync_CreatesJob()
    {
        // Arrange
        var mockRepo = new Mock<IJobRepository>();
        var service = new JobService(mockRepo.Object);
        
        // Act
        var result = await service.CreateJobAsync(...);
        
        // Assert
        mockRepo.Verify(r => r.AddAsync(It.IsAny<Job>()), Times.Once);
    }
}
```

### Integration Testing

#### Database Tests
```csharp
public class JobRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    
    public JobRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
    }
    
    [Fact]
    public async Task AddAsync_AddsJobToDatabase()
    {
        // Arrange
        var repository = new JobRepository(_context);
        var job = new Job { JobName = "Test" };
        
        // Act
        await repository.AddAsync(job);
        
        // Assert
        Assert.Equal(1, await _context.Jobs.CountAsync());
    }
}
```

### End-to-End Testing

#### Manual Test Scenarios
1. **Create Complete Bid:**
   - Create contractor
   - Create fixtures
   - Create bid items
   - Create job with fixtures and options
   - Verify total calculation
   - Export to PDF

2. **Edit Existing Bid:**
   - Open job
   - Modify quantities
   - Add/remove options
   - Verify recalculation
   - Save changes

3. **Data Management:**
   - Import bid items from CSV
   - Clone existing job
   - Delete and restore items
   - Export data backup

#### Automated E2E (Future)
- Playwright or Selenium for UI testing
- Test critical user journeys
- Run in CI pipeline

---

## Deployment Strategy

### Local Development
```bash
# Run locally with SQLite
cd src/PlumbingBiddingTool.Web
dotnet run

# Access at https://localhost:5001
```

### Staging/Production Deployment

#### Option 1: Azure App Service
```bash
# Publish to Azure
dotnet publish -c Release
# Deploy using Azure CLI or Visual Studio publish
```

#### Option 2: Docker Container
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PlumbingBiddingTool.Web.dll"]
```

#### Configuration Management
- Development: `appsettings.Development.json`
- Production: Environment variables or Azure Key Vault
- Connection strings: Stored securely, never in source control

---

## Maintenance & Support

### Regular Maintenance Tasks

#### Weekly
- Review error logs
- Monitor performance metrics
- Check for database growth
- Review user feedback

#### Monthly
- Update NuGet packages
- Security scan
- Backup verification
- Performance optimization review

#### Quarterly
- Major feature releases
- Pricing library updates
- User training sessions
- Roadmap planning

### Monitoring & Alerts

#### Key Metrics to Track
- Application availability (uptime)
- Response time (p50, p95, p99)
- Error rate
- Active users
- Database size
- Number of jobs created per day

#### Alert Thresholds
- Response time > 3 seconds
- Error rate > 1%
- Disk usage > 80%
- CPU usage > 70% sustained

---

## Using GitHub Copilot Effectively

### Best Practices

#### 1. Context is Key
- Keep related files open in editor
- Write clear comments before generating code
- Use descriptive variable and method names

#### 2. Test-Driven with Copilot
```csharp
// Write test first (Copilot helps)
[Fact]
public void Job_TotalCost_ShouldIncludeAllItems()
{
    // Generate test with Copilot
}

// Then implement feature
public decimal TotalCost => /* Let Copilot suggest */
```

#### 3. Effective Prompts
```csharp
// ✅ Good: Specific and clear
// Create a method to calculate job total cost including fixtures and options

// ❌ Poor: Vague
// Write code for total
```

#### 4. Boilerplate Generation
```csharp
// Create full CRUD repository implementation for Job entity with Entity Framework
public class JobRepository : IJobRepository
{
    // Copilot generates all methods
}
```

#### 5. Test Data Generation
```csharp
// Generate test data for Job with 5 fixtures and 3 options
private Job CreateTestJob()
{
    // Copilot creates comprehensive test data
}
```

### Example Prompts for Common Tasks

#### Creating Entities
"Create a Job entity with properties: Id, JobName, Status, ContractorId, collections for JobFixtureItems and JobOptions, and a calculated TotalCost property"

#### Creating Services
"Implement JobService with methods to create, update, delete, and get jobs including all related fixtures and options"

#### Creating Blazor Components
"Create a Blazor component for creating a new job with contractor selection, fixture selection with quantities, and custom options input"

#### Writing Tests
"Generate xUnit tests for JobService.CreateJobAsync method covering success case, validation failures, and exception handling"

---

## Risk Management

### Technical Risks

| Risk | Impact | Mitigation |
|------|--------|----------|
| Data loss | High | Regular backups, transaction management |
| Performance degradation | Medium | Indexing, query optimization, caching |
| Security vulnerabilities | High | Regular updates, security scanning, code review |
| Scalability issues | Medium | Database migration to enterprise DB, consider caching |

### Process Risks

| Risk | Impact | Mitigation |
|------|--------|----------|
| Scope creep | Medium | Stick to roadmap, prioritize ruthlessly |
| Technical debt accumulation | Medium | Regular refactoring, maintain test coverage |
| Single developer dependency | High | Good documentation, code clarity |
| User adoption issues | Medium | Training, user feedback loops |

---

## Success Metrics

### Development KPIs

| Metric | Target | How to Measure |
|--------|--------|---------------|
| Code coverage | >70% | Test runner reports |
| Build success rate | >95% | CI/CD metrics |
| Average bug fix time | <48 hours | Issue tracking |
| Feature delivery | 80% on-time | Project tracking |

### Application KPIs

| Metric | Target | How to Measure |
|--------|--------|---------------|
| Uptime | >99% | Monitoring tools |
| Response time | <2s (p95) | Application Insights |
| Jobs created per week | Growth trend | Database analytics |
| User satisfaction | >4.5/5 | User surveys |

---

## Resources & References

### Documentation
- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Blazor Documentation](https://docs.microsoft.com/aspnet/core/blazor)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

### Libraries to Consider
- **PDF Generation:** QuestPDF, DinkToPdf
- **Excel Export:** ClosedXML, EPPlus
- **Logging:** Serilog
- **Testing:** xUnit, Moq, FluentAssertions
- **Mapping:** AutoMapper
- **Validation:** FluentValidation

---

## Appendix: Quick Reference

### Common Commands

```bash
# Build
dotnet build

# Run tests
dotnet test

# Run app
cd src/PlumbingBiddingTool.Web
dotnet run

# Create migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Add package
dotnet add package PackageName
```

### File Locations
- Domain Entities: `src/PlumbingBiddingTool.Domain/Entities/`
- Services: `src/PlumbingBiddingTool.Application/`
- Repositories: `src/PlumbingBiddingTool.Infrastructure/Repositories/`
- Pages: `src/PlumbingBiddingTool.Web/Components/Pages/`
- Database: `src/PlumbingBiddingTool.Infrastructure/plumbingbidding.db`

---

**Document Version History**

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | February 2025 | Development Team | Initial development plan for Plumbing Bidding Tool |
