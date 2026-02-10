# Product Requirements Document

**Product Name:** Plumbing Bidding Tool  
**Version:** 1.0  
**Date:** February 2025  
**Status:** In Development

---

## 1. Executive Summary

### One-Sentence Overview
A web-based tool that streamlines the plumbing bidding process for new construction projects by allowing contractors to quickly create accurate bids using pre-configured fixture items, bid items, and customizable job options.

### Product Vision
Simplify and accelerate the plumbing bid creation process for new construction projects, reducing errors and improving profitability for plumbing contractors.

---

## 2. Problem Statement & Opportunity

### Current Challenges
- **Manual Bid Creation:** Plumbing contractors spend significant time manually calculating costs for new construction projects
- **Inconsistent Pricing:** Without a centralized system, pricing can vary across bids leading to profitability issues
- **Complex Calculations:** Tracking fixtures, bid items across multiple phases (Underground, Stack Out, Trim), and custom options is error-prone
- **Limited Visibility:** Difficulty tracking multiple jobs and bids for different contractors
- **Time-Consuming Process:** Creating detailed bids with all components and accurate totals requires extensive manual work

### Opportunity
Create a streamlined web application that provides:
- Pre-configured fixture and bid item libraries
- Automated cost calculations
- Multi-phase plumbing project support (Underground, Stack Out, Trim)
- Job tracking and management
- Contractor-specific bid organization

---

## 3. Target Users

### Primary Users
- **Plumbing Contractors:** Small to medium-sized plumbing companies bidding on new construction residential and commercial projects
- **Estimators:** Team members responsible for creating bids and quotes
- **Business Owners:** Plumbing company owners managing multiple contractors and projects

### User Personas

**Persona 1: Small Contractor**
- Runs a 2-5 person plumbing operation
- Bids on 10-20 residential new construction projects per month
- Needs quick, accurate bids to remain competitive
- Values simplicity and ease of use

**Persona 2: Estimator at Medium-Sized Company**
- Creates 30+ bids per month
- Manages multiple contractors/builders relationships
- Requires consistency and accuracy
- Needs to track bid history and job status

---

## 4. Core Value Proposition

> **"Create accurate plumbing bids for new construction in minutes, not hours - with automated calculations, pre-configured items, and organized job tracking."**

### Key Benefits
1. **Speed:** Reduce bid creation time from hours to minutes
2. **Accuracy:** Automated calculations eliminate manual errors
3. **Consistency:** Standardized pricing across all bids
4. **Organization:** Track all jobs and contractors in one place
5. **Flexibility:** Support custom job options for unique requirements

---

## 5. Product Goals & Success Metrics

### Business Goals
- Reduce bid creation time by 70%+
- Improve bid accuracy and reduce pricing errors
- Support contractors in winning more profitable bids
- Enable scaling to 100+ active jobs

### Success Metrics

| Metric | Target | Measurement Method |
|--------|--------|-------------------|
| Average bid creation time | <15 minutes | User feedback/analytics |
| User adoption | 5+ active contractors in first 3 months | Database records |
| Bids created per month | 50+ bids/month after 3 months | Database records |
| Calculation accuracy | 100% (vs. manual) | Testing/validation |
| User satisfaction | 4.5/5 stars | User surveys |

---

## 6. Feature Scope

### Core Features (MVP - Current Implementation)

#### 6.1 Fixture Item Management
- **Description:** Pre-configured plumbing fixtures with associated bid items
- **Capabilities:**
  - View all fixture items with calculated prices
  - Each fixture automatically includes its component bid items
  - Fixture price = sum of all associated bid items
- **User Story:** "As an estimator, I want to select fixtures (e.g., 'Full Bath') and automatically get all associated components priced correctly"

#### 6.2 Bid Item Management
- **Description:** Individual plumbing components with pricing
- **Capabilities:**
  - Create, read, update, delete bid items
  - Categorize by phase (Underground, Stack Out, Trim)
  - Categorize by type (Sewer, Water, Gas)
  - Set individual pricing per item
- **User Story:** "As a business owner, I want to maintain a library of plumbing components with current pricing"

#### 6.3 Contractor Management
- **Description:** Track and organize multiple contractor relationships
- **Capabilities:**
  - Create and manage contractor profiles
  - View all jobs associated with each contractor
  - Track contractor-specific job history
- **User Story:** "As an estimator, I want to organize my bids by contractor/builder so I can manage multiple relationships"

#### 6.4 Job Creation & Management
- **Description:** Create detailed plumbing bids for new construction projects
- **Capabilities:**
  - Create new jobs linked to contractors
  - Select fixtures with quantity inputs
  - Add custom job options (additional items not in fixture library)
  - Automatic total cost calculation
  - Job status tracking (Open/In Progress/Completed/Closed)
  - View job details with complete breakdown
  - Edit existing jobs
- **User Story:** "As a contractor, I want to create a bid by selecting fixtures, setting quantities, and adding custom options, then see the total automatically calculated"

#### 6.5 Multi-Phase Support
- **Description:** Support three phases of plumbing construction
- **Phases:**
  - **Underground:** Sewer, water, and gas lines beneath slab
  - **Stack Out:** Vertical plumbing stacks and rough-in
  - **Trim:** Fixture installation and finish work
- **User Story:** "As an estimator, I need to organize my bid items by construction phase to align with how projects are actually built"

#### 6.6 Cost Calculations
- **Description:** Automatic, accurate bid total calculations
- **Capabilities:**
  - Calculate fixture subtotal: Sum of (fixture price × quantity)
  - Calculate options subtotal: Sum of (option price × quantity)
  - Calculate job total: Fixtures + Options
  - Real-time updates as quantities change
- **User Story:** "As a contractor, I want the total automatically calculated so I can see my bid amount without manual math"

### Out of Scope (Future Enhancements)
- PDF/Excel export of bids
- Email bid delivery
- Customer/client management
- Profit margin analysis and recommendations
- Historical pricing trends
- Material supplier integration
- Mobile app
- Multi-user access control
- Bid versioning
- Won/Lost bid tracking with reasons
- Payment tracking
- Integration with accounting software

---

## 7. Technical Architecture

### Technology Stack
- **Frontend:** Blazor Server (.NET 8)
- **Backend:** ASP.NET Core (.NET 8)
- **Database:** SQLite (currently), ready for migration to SQL Server/PostgreSQL
- **Architecture Pattern:** Clean Architecture
  - **Domain Layer:** Entities, Value Objects, Interfaces
  - **Application Layer:** Services, Use Cases, Business Logic
  - **Infrastructure Layer:** Data Access, Repositories, External Services
  - **Web Layer:** Blazor Components, Pages, UI

### Domain Model

#### Core Entities
1. **Contractor**
   - Id, Name
   - Collection of Jobs

2. **Job**
   - Id, JobName, Status, ContractorId
   - Collection of JobFixtureItems
   - Collection of JobOptions
   - Calculated TotalCost

3. **FixtureItem**
   - Id, Name
   - Collection of BidItems
   - Calculated Price (sum of bid items)

4. **BidItem**
   - Id, Name, Price, Phase, ItemType

5. **JobFixtureItem** (Join entity)
   - JobId, FixtureItemId, Quantity, Price

6. **JobOption** (Custom additions)
   - JobId, Name, Quantity, Price

#### Enumerations
- **Phase:** Underground, StackOut, Trim
- **ItemType:** Sewer, Water, Gas
- **JobStatus:** Open, InProgress, Completed, Closed

### Data Flow
```
User → Blazor Component → Application Service → Repository → Database
                                ↓
                          Domain Logic & Validation
```

---

## 8. User Experience & Workflows

### Workflow 1: Creating a New Bid

1. **Navigate to Create Job page**
2. **Select Contractor** from dropdown (or create new)
3. **Enter Job Name** (e.g., "Lot 42 - Maple Street")
4. **Select Fixtures:**
   - Browse fixture list with prices
   - Enter quantity for each fixture needed
   - See fixture subtotal update
5. **Add Custom Job Options** (if needed):
   - Add name (e.g., "Gas line extension")
   - Enter quantity and price
6. **Review Total Cost**
   - See automatic calculation
   - Fixtures subtotal + Options subtotal = Total
7. **Submit Job**
8. **View Confirmation** with job details

### Workflow 2: Managing Fixture Library

1. **Navigate to Fixture Items**
2. **View All Fixtures** with calculated prices
3. **Each fixture shows:**
   - Name
   - Associated bid items
   - Total calculated price
4. **Note:** Fixture management uses the existing bid items

### Workflow 3: Managing Bid Items

1. **Navigate to Bid Items**
2. **View/Filter** by Phase or ItemType
3. **Create New Bid Item:**
   - Enter name
   - Set price
   - Select phase
   - Select type
4. **Edit Existing Item** to update pricing
5. **Save Changes**

### Workflow 4: Reviewing Jobs

1. **Navigate to Jobs**
2. **View Jobs List:**
   - All jobs with contractor, status, total
   - Filter/search capabilities
3. **Select Job** to view details:
   - Complete fixture breakdown
   - Custom options
   - Total cost
4. **Edit Job** if needed
5. **Update Status** as work progresses

---

## 9. User Interface Requirements

### Design Principles
- **Clean & Simple:** Minimize clutter, focus on essential information
- **Responsive:** Work on desktop, tablet, and mobile
- **Fast:** Quick page loads and responsive interactions
- **Intuitive:** Follow standard web conventions
- **Data-Focused:** Present information clearly with good visual hierarchy

### Key UI Components
1. **Navigation:** Clear menu structure (Jobs, Contractors, Fixtures, Bid Items, Settings)
2. **Tables:** Sortable, filterable lists for all entities
3. **Forms:** Simple, validated input forms
4. **Cards:** Summary cards for displaying grouped information
5. **Modals:** For confirmations and quick actions

### Color Scheme & Branding
- Professional appearance suitable for contractors
- Clear visual feedback for actions
- Status indicators for job states

---

## 10. Non-Functional Requirements

### Performance
- Page load time: <2 seconds
- Support for 100+ active jobs
- Concurrent user support: 5-10 simultaneous users
- Database query optimization for large datasets

### Security
- HTTPS for all communications
- Input validation and sanitization
- SQL injection prevention (using Entity Framework parameterized queries)
- Future: Authentication and authorization

### Reliability
- 99% uptime target
- Automatic database backups
- Error handling and logging
- Graceful degradation on failures

### Usability
- Intuitive interface requiring minimal training
- Consistent UI patterns throughout
- Helpful error messages
- Responsive design for various screen sizes

### Maintainability
- Clean Architecture for easy updates
- Well-documented code
- Separation of concerns
- Unit test coverage for critical paths

---

## 11. Development Roadmap

### Phase 1: MVP Foundation (Completed)
- ✅ Clean Architecture structure
- ✅ Domain entities and relationships
- ✅ Database context with SQLite
- ✅ Basic CRUD repositories
- ✅ Core application services
- ✅ Blazor components for all main features
- ✅ Job creation with fixtures and options
- ✅ Automatic cost calculations
- ✅ Multi-phase bid item support

### Phase 2: Enhancements (Next 1-3 Months)
- [ ] PDF bid export
- [ ] Enhanced search and filtering
- [ ] Bid templates for common job types
- [ ] Job cloning/duplication
- [ ] Improved reporting (cost breakdowns by phase)
- [ ] Settings page for configuration
- [ ] Data validation improvements
- [ ] User testing and feedback incorporation

### Phase 3: Advanced Features (3-6 Months)
- [ ] Multi-user support with authentication
- [ ] Role-based access control
- [ ] Bid versioning and history
- [ ] Won/Lost tracking with analytics
- [ ] Email notifications
- [ ] Migration to SQL Server/PostgreSQL
- [ ] API for integrations
- [ ] Mobile app consideration

### Phase 4: Scale & Integration (6-12 Months)
- [ ] Integration with accounting software (QuickBooks)
- [ ] Material supplier pricing integration
- [ ] Advanced analytics and reporting
- [ ] Profit margin optimization tools
- [ ] Historical pricing analysis
- [ ] Market competitive analysis

---

## 12. Risks & Mitigation Strategies

| Risk | Impact | Probability | Mitigation |
|------|--------|-------------|------------|
| Incomplete fixture library | Medium | Medium | Start with most common fixtures; add incrementally |
| Pricing becomes outdated | High | High | Regular pricing review process; bulk update capability |
| User adoption resistance | Medium | Medium | Focus on ease of use; provide training; show time savings |
| Performance with large datasets | Medium | Low | Database optimization; implement pagination; indexing |
| Data loss | High | Low | Regular backups; data export capability |
| Feature creep | Medium | High | Stick to roadmap; document future features separately |

---

## 13. Success Criteria for Launch

### Must Have
- [ ] All CRUD operations working for Contractors, Jobs, Fixtures, and Bid Items
- [ ] Accurate automatic calculations verified
- [ ] Clean, professional UI
- [ ] Responsive design working on desktop and tablet
- [ ] No critical bugs
- [ ] Basic error handling in place
- [ ] Database migrations working

### Should Have
- [ ] User documentation/guide
- [ ] 20+ fixtures in library
- [ ] 50+ bid items covering common scenarios
- [ ] Comprehensive testing of all major workflows

### Nice to Have
- [ ] PDF export capability
- [ ] Search functionality
- [ ] Keyboard shortcuts for power users

---

## 14. Future Considerations

### Scalability
- Multi-tenant architecture for SaaS offering
- Cloud deployment (Azure/AWS)
- Database migration to enterprise solution
- Performance optimization for 1000+ jobs

### Monetization (if applicable)
- Subscription model for contractors
- Per-bid pricing
- Premium features (advanced reporting, integrations)
- White-label option for larger companies

### Integration Opportunities
- Accounting software (QuickBooks, Xero)
- Material suppliers for real-time pricing
- Project management tools
- CRM systems

---

## Appendix A: Glossary

- **Fixture:** A complete plumbing component (e.g., toilet, sink, bath) that includes multiple bid items
- **Bid Item:** An individual plumbing component with a specific price (e.g., "2" ABS trap", "Supply stop")
- **Job:** A complete plumbing bid for a new construction project
- **Job Option:** Custom line items added to a job that aren't in the fixture library
- **Phase:** Construction stage (Underground, Stack Out, or Trim)
- **Contractor:** Builder or general contractor requesting the plumbing bid
- **Stack Out:** The rough-in phase where vertical plumbing stacks are installed

---

## Appendix B: Database Schema Overview

### Tables
- **Contractors** (Id, Name)
- **Jobs** (Id, JobName, Status, ContractorId)
- **FixtureItems** (Id, Name)
- **BidItems** (Id, Name, Price, Phase, ItemType)
- **JobFixtureItems** (Id, JobId, FixtureItemId, Quantity, Price)
- **JobOptions** (Id, JobId, Name, Quantity, Price)
- **FixtureItemBidItems** (FixtureItemId, BidItemId) - Join table

### Relationships
- Contractor 1→N Jobs
- Job 1→N JobFixtureItems
- Job 1→N JobOptions
- FixtureItem 1→N JobFixtureItems
- FixtureItem N→N BidItems (through join table)

---

**Document Version History**

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | February 2025 | Development Team | Initial PRD for Plumbing Bidding Tool |
