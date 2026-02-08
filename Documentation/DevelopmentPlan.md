# Development Plan - Plumbing Fixture Identifier

**Project:** Plumbing Fixture Identifier (PFI Service)  
**Version:** MVP 1.0  
**Team:** One Developer + GitHub Copilot  
**Timeline:** 4-6 Weeks  
**Last Updated:** February 2025

---

## Table of Contents

1. [Overview](#overview)
2. [Development Environment Setup](#development-environment-setup)
3. [Week-by-Week Plan](#week-by-week-plan)
4. [Implementation Guidelines](#implementation-guidelines)
5. [Testing Strategy](#testing-strategy)
6. [Deployment Process](#deployment-process)
7. [Quality Checklist](#quality-checklist)

---

## Overview

This development plan outlines the step-by-step implementation approach for building the Plumbing Fixture Identifier MVP using a one developer + GitHub Copilot team. The plan follows Clean Architecture principles and prioritizes rapid iteration with continuous testing.

### Development Philosophy
- **Clean Architecture First:** Build proper separation of concerns from day one
- **Test-Driven Development:** Write tests alongside implementation
- **Incremental Delivery:** Deploy and test each component as it's completed
- **Copilot Integration:** Use GitHub Copilot for boilerplate, tests, and standard patterns
- **Fail Fast:** Test with real services early to catch integration issues

---

## Development Environment Setup

### Prerequisites

1. **Development Tools**
   - Visual Studio 2022 or VS Code with C# extensions
   - .NET 8 SDK
   - Git
   - Postman or similar API testing tool
   - Azure Storage Explorer (for local testing)
   - Azure Functions Core Tools v4

2. **Cloud Accounts & Services**
   - Azure subscription (free tier sufficient for testing)
   - Twilio account with phone number purchased
   - QuickBooks Online Developer account
   - GitHub account (for version control and Copilot)

3. **Local Configuration**
   ```bash
   # Clone the repository
   git clone https://github.com/MasterSkriptor/PlumbingBiddingTool.git
   cd PlumbingBiddingTool
   
   # Restore dependencies
   dotnet restore
   
   # Build solution
   dotnet build
   
   # Run tests
   dotnet test
   ```

4. **Create local.settings.json**
   ```json
   {
     "IsEncrypted": false,
     "Values": {
       "AzureWebJobsStorage": "UseDevelopmentStorage=true",
       "FUNCTIONS_WORKER_RUNTIME": "dotnet",
       "TwilioAccountSid": "your-twilio-sid",
       "TwilioAuthToken": "your-twilio-token",
       "TwilioPhoneNumber": "+1xxxxxxxxxx",
       "AzureCustomVisionPredictionKey": "your-prediction-key",
       "AzureCustomVisionPredictionEndpoint": "your-endpoint",
       "AzureCustomVisionProjectId": "your-project-id",
       "QuickBooksClientId": "your-client-id",
       "QuickBooksClientSecret": "your-client-secret",
       "QuickBooksEnvironment": "sandbox",
       "QueryPriceUsd": "0.75"
     }
   }
   ```

---

## Week-by-Week Plan

### Week 1: Foundation & Project Setup

**Goal:** Establish clean architecture structure and basic Twilio integration

#### Day 1-2: Project Structure & Domain Layer
- [ ] **Task 1.1:** Verify existing clean architecture structure
  - Review Domain, Application, Infrastructure, Web layers
  - Ensure proper dependency flow (Domain ← Application ← Infrastructure ← Web/Functions)
  
- [ ] **Task 1.2:** Define Domain Entities
  ```
  Create domain entities in PlumbingBiddingTool.Domain:
  - FixtureIdentification (Id, PhoneNumber, ImageUrl, Manufacturer, Model, Confidence, Timestamp)
  - Customer (Id, PhoneNumber, Name, QuickBooksId)
  - Invoice (Id, CustomerId, Amount, QueryId, Status, InvoiceDate)
  ```
  - **Copilot Prompt:** "Generate domain entities for fixture identification service with manufacturer, model, and confidence score"
  
- [ ] **Task 1.3:** Define Value Objects
  ```
  - PhoneNumber (with validation)
  - Money (with currency support)
  - Confidence (0.0 to 1.0 range)
  ```

- [ ] **Task 1.4:** Write Domain Unit Tests
  - Test entity validation logic
  - Test value object constraints
  - **Copilot Prompt:** "Generate xUnit tests for PhoneNumber value object validation"

#### Day 3-4: Application Layer & Use Cases
- [ ] **Task 1.5:** Define Application Interfaces (Ports)
  ```
  Create interfaces in PlumbingBiddingTool.Application.Contracts:
  - IMessagingService (SendSms, ReceiveMms)
  - IImageRecognitionService (IdentifyFixture)
  - IBillingService (CreateInvoice, GetOrCreateCustomer)
  - IFixtureRepository (Save, GetById, GetByPhoneNumber)
  ```

- [ ] **Task 1.6:** Implement Use Cases
  ```
  Create use cases in PlumbingBiddingTool.Application.UseCases:
  - ProcessFixtureIdentificationRequest
    - Input: PhoneNumber, ImageUrl
    - Output: FixtureIdentification result
    - Steps: Validate → Identify → Respond → Bill
  ```
  - **Copilot Prompt:** "Generate use case for processing fixture identification with image recognition and billing"

- [ ] **Task 1.7:** Add Application Layer Tests
  - Mock all dependencies
  - Test happy path and error scenarios
  - Test confidence threshold logic (>0.8 = success, <0.8 = retry message)

#### Day 5-7: Twilio Integration & Basic Function
- [ ] **Task 1.8:** Implement Twilio Service
  ```
  In PlumbingBiddingTool.Infrastructure.Messaging:
  - TwilioService : IMessagingService
  - Send SMS (use Twilio .NET SDK)
  - Parse MMS webhook payload
  - Extract image URL from webhook
  ```
  - **Copilot Prompt:** "Implement Twilio service for sending SMS and receiving MMS webhooks"

- [ ] **Task 1.9:** Create Azure Function for Webhook
  ```
  Create in PlumbingBiddingTool.Functions:
  - TwilioWebhookFunction [HttpTrigger]
  - Accept POST from Twilio
  - Parse form data
  - Call ProcessFixtureIdentificationRequest use case
  - Return HTTP 200
  ```

- [ ] **Task 1.10:** Local Testing Setup
  - Use ngrok to expose local endpoint
  - Configure Twilio webhook to ngrok URL
  - Send test MMS and verify reception
  - Verify SMS response (mock AI for now)

**Week 1 Deliverables:**
- ✅ Clean architecture structure implemented
- ✅ Domain and Application layers complete with tests
- ✅ Twilio service functional
- ✅ Azure Function receives MMS webhooks
- ✅ Can send SMS responses
- ✅ All tests passing

---

### Week 2: Azure Custom Vision Integration

**Goal:** Implement AI image recognition capability

#### Day 8-9: Azure Custom Vision Setup
- [ ] **Task 2.1:** Prepare Training Dataset
  - Collect 200-500 images of common plumbing fixtures
  - Organize by manufacturer and model
  - Target brands: Kohler, American Standard, Delta, Moen, Mansfield, etc.
  - Ensure variety: different angles, lighting conditions

- [ ] **Task 2.2:** Create Azure Custom Vision Project
  - Create new project in Azure portal (Object Detection or Classification)
  - Upload and tag training images
  - Tag format: "Manufacturer - Model" (e.g., "Kohler - Wellworth K-3987")
  - Train initial model (can take 10-30 minutes)

- [ ] **Task 2.3:** Test Model Accuracy
  - Test with validation images (separate from training set)
  - Verify confidence scores
  - Iterate on tagging if accuracy <80%

#### Day 10-12: Custom Vision Integration
- [ ] **Task 2.4:** Implement Custom Vision Service
  ```
  In PlumbingBiddingTool.Infrastructure.AI:
  - AzureCustomVisionService : IImageRecognitionService
  - Download image from URL
  - Call Custom Vision Prediction API
  - Parse response (manufacturer, model, confidence)
  - Handle low confidence scenarios
  ```
  - **Copilot Prompt:** "Implement Azure Custom Vision service to identify plumbing fixtures from images"

- [ ] **Task 2.5:** Add Response Formatting
  ```
  Create SMS response templates:
  - High confidence: "Identified: {Manufacturer} {Model}. Invoice sent to your QuickBooks."
  - Low confidence: "Unable to identify. Please send a clearer, closer photo of the manufacturer label."
  - Error: "Sorry, we encountered an error. Please try again."
  ```

- [ ] **Task 2.6:** Integration Testing
  - Test with real fixture images
  - Verify confidence threshold logic
  - Test edge cases (no fixture, multiple fixtures, blurry images)
  - Measure response time (should be <10 seconds)

#### Day 13-14: Performance & Reliability
- [ ] **Task 2.7:** Add Image Preprocessing (if needed)
  - Resize large images to reduce processing time
  - Basic quality checks
  - Format conversion if necessary

- [ ] **Task 2.8:** Implement Error Handling
  - Retry logic for transient failures
  - Graceful degradation on service outage
  - Log all prediction attempts with confidence scores

- [ ] **Task 2.9:** Add Monitoring
  - Log to Application Insights
  - Track identification success rate
  - Track average confidence scores
  - Track response times

**Week 2 Deliverables:**
- ✅ Azure Custom Vision model trained (≥80% accuracy)
- ✅ Image recognition service implemented
- ✅ End-to-end flow working: MMS → Identify → SMS response
- ✅ Proper error handling and logging
- ✅ Performance meets <10 second target

---

### Week 3: QuickBooks Integration

**Goal:** Implement automated billing via QuickBooks Online API

#### Day 15-16: QuickBooks Setup & Authentication
- [ ] **Task 3.1:** QuickBooks Developer Setup
  - Create app in Intuit Developer Portal
  - Configure OAuth 2.0 redirect URLs
  - Get Client ID and Client Secret
  - Set up sandbox company for testing

- [ ] **Task 3.2:** Implement OAuth Flow
  ```
  In PlumbingBiddingTool.Infrastructure.Billing:
  - QuickBooksAuthService
  - Implement OAuth 2.0 authorization flow
  - Store/refresh access tokens
  - Handle token expiration (401 responses)
  ```
  - **Copilot Prompt:** "Implement OAuth 2.0 authentication for QuickBooks Online API with token refresh"

- [ ] **Task 3.3:** Create Admin Endpoint for Authorization
  ```
  - Create simple endpoint to initiate OAuth
  - Handle callback and exchange code for tokens
  - Store tokens securely (for MVP, in configuration; later, Key Vault)
  ```

#### Day 17-19: Customer & Invoice Management
- [ ] **Task 3.4:** Implement Customer Service
  ```
  In PlumbingBiddingTool.Infrastructure.Billing:
  - QuickBooksCustomerService : IBillingService
  - GetOrCreateCustomer(phoneNumber)
    - Search by DisplayName or phone
    - Create if not exists
    - Return QuickBooks Customer ID
  ```
  - **Copilot Prompt:** "Implement QuickBooks customer creation and lookup by phone number using Intuit SDK"

- [ ] **Task 3.5:** Implement Invoice Generation
  ```
  - CreateInvoice(customerId, amount, description)
    - Create invoice with line item for fixture identification
    - Set amount to $0.75
    - Set due date (e.g., Net 15)
    - Save invoice
    - Return invoice number
  ```

- [ ] **Task 3.6:** Integration Testing
  - Test full customer creation flow in sandbox
  - Verify invoice appears in QuickBooks sandbox
  - Test duplicate customer handling
  - Test error scenarios (invalid customer, API limits)

#### Day 20-21: End-to-End Testing
- [ ] **Task 3.7:** Complete Integration Test
  ```
  Full flow test:
  1. Send MMS with fixture photo
  2. Verify image recognition
  3. Verify SMS response
  4. Verify customer created in QuickBooks
  5. Verify invoice generated
  6. Check all logs and metrics
  ```

- [ ] **Task 3.8:** Idempotency Implementation
  - Ensure duplicate MMS doesn't create duplicate invoices
  - Track processed queries in repository
  - Add duplicate detection logic

- [ ] **Task 3.9:** Cost Validation
  - Calculate actual cost per query:
    - Twilio MMS receive: ~$0.005
    - Twilio SMS send: ~$0.0075
    - Azure Custom Vision: ~$0.001 per prediction
    - QuickBooks API: Free tier sufficient
    - Total: ~$0.014 per query (well under $0.20 target)

**Week 3 Deliverables:**
- ✅ QuickBooks OAuth working
- ✅ Customer creation/lookup functional
- ✅ Invoice generation working
- ✅ End-to-end flow complete and tested
- ✅ Idempotency implemented
- ✅ Cost per query validated

---

### Week 4: Deployment & Testing

**Goal:** Deploy to Azure and validate with real usage

#### Day 22-23: Azure Deployment
- [ ] **Task 4.1:** Prepare for Deployment
  - Review all configuration settings
  - Ensure all secrets are externalized
  - Create Azure resources:
    - Function App (Consumption plan)
    - Application Insights
    - Storage Account (for function state)

- [ ] **Task 4.2:** Deploy to Azure
  ```bash
  # Using Azure CLI or VS publish
  func azure functionapp publish PlumbingFixtureIdentifier
  ```
  - Verify Function App is running
  - Configure application settings (copy from local.settings.json)
  - Test with Azure endpoint

- [ ] **Task 4.3:** Configure Twilio Webhook
  - Update Twilio webhook URL to Azure endpoint
  - Test MMS reception on Azure
  - Verify SMS responses working

#### Day 24-25: Domain Configuration & Testing
- [ ] **Task 4.4:** DNS Configuration
  - Configure api.excels.com in Network Solutions
  - Create CNAME to Azure Function App
  - Wait for DNS propagation
  - Test with custom domain

- [ ] **Task 4.5:** Production QuickBooks Setup
  - Switch from sandbox to production environment
  - Re-authenticate with production QuickBooks company
  - Create test invoice to verify

- [ ] **Task 4.6:** End-to-End Production Test
  - Send real MMS from actual phone
  - Verify complete flow in production
  - Check Application Insights for telemetry
  - Verify invoice in production QuickBooks

#### Day 26-28: User Testing & Iteration
- [ ] **Task 4.7:** Internal Testing
  - Test with 10-20 different fixture images
  - Document identification accuracy
  - Track response times
  - Note any failures or issues

- [ ] **Task 4.8:** Address Issues
  - Fix any bugs discovered
  - Retrain AI model if accuracy is low
  - Optimize response messages
  - Improve error handling based on real scenarios

- [ ] **Task 4.9:** Documentation
  - Create user guide (how to use the service)
  - Document setup/deployment process
  - Create troubleshooting guide
  - Document architecture decisions

**Week 4 Deliverables:**
- ✅ Application deployed to Azure
- ✅ Custom domain configured
- ✅ Production QuickBooks integrated
- ✅ 10+ successful end-to-end tests
- ✅ Documentation complete
- ✅ Ready for beta users

---

### Week 5-6: Beta Testing & Refinement

**Goal:** Validate with real plumbers and iterate based on feedback

#### Week 5: Beta User Testing
- [ ] **Task 5.1:** Recruit Beta Testers
  - Reach out to 5-10 local plumbers
  - Provide clear instructions
  - Set expectations (MVP, may have issues)

- [ ] **Task 5.2:** Monitor Usage
  - Watch Application Insights in real-time
  - Track key metrics:
    - Queries per day
    - Identification success rate
    - Average confidence score
    - Response time
    - Billing success rate

- [ ] **Task 5.3:** Collect Feedback
  - Follow up with each beta user
  - Ask about:
    - Ease of use
    - Accuracy of results
    - Response time
    - Value proposition
    - Price point

- [ ] **Task 5.4:** Analyze Failures
  - Review all low-confidence identifications
  - Collect failed images
  - Identify patterns (lighting, angle, fixture types)

#### Week 6: Iteration & Polish
- [ ] **Task 6.1:** Model Improvement
  - Add failed images to training set
  - Retrain Custom Vision model
  - Deploy updated model
  - Retest with previous failures

- [ ] **Task 6.2:** UX Improvements
  - Refine SMS response messages based on feedback
  - Add helpful tips (e.g., "For best results, photo the manufacturer label directly")
  - Consider welcome message for first-time users

- [ ] **Task 6.3:** Performance Optimization
  - Optimize image download/processing
  - Implement caching if applicable
  - Review and optimize API calls

- [ ] **Task 6.4:** Launch Preparation
  - Final security review
  - Verify all error handling
  - Confirm cost projections
  - Prepare launch communication
  - Set up monitoring alerts

**Week 5-6 Deliverables:**
- ✅ 50-100 real queries processed
- ✅ ≥80% identification accuracy achieved
- ✅ User feedback collected and analyzed
- ✅ Major issues resolved
- ✅ Ready for broader launch

---

## Implementation Guidelines

### Clean Architecture Principles

```
PlumbingBiddingTool/
├── src/
│   ├── PlumbingBiddingTool.Domain/
│   │   ├── Entities/
│   │   │   ├── FixtureIdentification.cs
│   │   │   ├── Customer.cs
│   │   │   └── Invoice.cs
│   │   ├── ValueObjects/
│   │   │   ├── PhoneNumber.cs
│   │   │   ├── Money.cs
│   │   │   └── Confidence.cs
│   │   └── Exceptions/
│   │       └── DomainException.cs
│   │
│   ├── PlumbingBiddingTool.Application/
│   │   ├── Contracts/
│   │   │   ├── IMessagingService.cs
│   │   │   ├── IImageRecognitionService.cs
│   │   │   ├── IBillingService.cs
│   │   │   └── IFixtureRepository.cs
│   │   ├── UseCases/
│   │   │   ├── ProcessFixtureIdentificationRequest.cs
│   │   │   └── GetFixtureIdentificationHistory.cs
│   │   ├── DTOs/
│   │   │   ├── FixtureIdentificationRequest.cs
│   │   │   └── FixtureIdentificationResponse.cs
│   │   └── Exceptions/
│   │       └── ApplicationException.cs
│   │
│   ├── PlumbingBiddingTool.Infrastructure/
│   │   ├── Messaging/
│   │   │   └── TwilioService.cs
│   │   ├── AI/
│   │   │   └── AzureCustomVisionService.cs
│   │   ├── Billing/
│   │   │   ├── QuickBooksAuthService.cs
│   │   │   └── QuickBooksCustomerService.cs
│   │   ├── Persistence/
│   │   │   └── InMemoryFixtureRepository.cs (for MVP)
│   │   └── Configuration/
│   │       └── ServiceConfiguration.cs
│   │
│   └── PlumbingBiddingTool.Functions/
│       ├── TwilioWebhookFunction.cs
│       ├── QuickBooksAuthFunction.cs (for OAuth callback)
│       ├── host.json
│       └── local.settings.json
│
└── tests/
    ├── PlumbingBiddingTool.Domain.Tests/
    ├── PlumbingBiddingTool.Application.Tests/
    └── PlumbingBiddingTool.Infrastructure.Tests/
```

### Dependency Rules
1. **Domain** has no dependencies (pure business logic)
2. **Application** depends only on Domain
3. **Infrastructure** depends on Application and Domain
4. **Functions** depends on all layers (composition root)

### Coding Standards
- Use C# 12 features where appropriate
- Follow .NET naming conventions
- Use dependency injection throughout
- Implement proper exception handling
- Add XML documentation for public APIs
- Use async/await for I/O operations
- Log all important operations

---

## Testing Strategy

### Unit Tests (70% coverage target)
- **Domain Tests:** Entity validation, value object constraints
- **Application Tests:** Use case logic with mocked dependencies
- **Infrastructure Tests:** Service implementations with mocked external APIs

### Integration Tests
- Test with real external services in sandbox/test mode
- Twilio test credentials
- QuickBooks sandbox
- Azure Custom Vision test project

### End-to-End Tests
- Manual testing with real phone and images
- Automated test suite using Twilio test numbers
- Verify complete flow: MMS → Recognition → Billing → Response

### Performance Tests
- Measure response time under various conditions
- Test with different image sizes/formats
- Verify scalability with concurrent requests

---

## Deployment Process

### Local Development
```bash
# Start Azure Functions locally
cd src/PlumbingBiddingTool.Functions
func start

# In another terminal, use ngrok for Twilio webhook
ngrok http 7071
```

### Azure Deployment
```bash
# Deploy using Azure Functions Core Tools
func azure functionapp publish PlumbingFixtureIdentifier

# Or use CI/CD pipeline in Azure DevOps
# Configure build and release pipelines
```

### Configuration Management
- **Local:** local.settings.json (not in source control)
- **Azure:** Application Settings in Function App
- **Future:** Azure Key Vault for secrets

### Monitoring & Logging
- **Application Insights:** Automatic telemetry
- **Custom Logging:** Structured logging with Serilog
- **Metrics Dashboard:** Track key performance indicators

---

## Quality Checklist

### Before Each Commit
- [ ] Code compiles without warnings
- [ ] All unit tests pass
- [ ] Code follows style guidelines
- [ ] Changes are minimal and focused
- [ ] Commit message is clear

### Before Merging to Main
- [ ] All tests pass (unit + integration)
- [ ] Code review completed
- [ ] Documentation updated
- [ ] No secrets in code
- [ ] Breaking changes documented

### Before Deployment
- [ ] All tests pass in staging
- [ ] Configuration verified
- [ ] Monitoring configured
- [ ] Rollback plan ready
- [ ] Stakeholders notified

### Before Launch
- [ ] Security review completed
- [ ] Performance validated
- [ ] Cost projections confirmed
- [ ] User documentation ready
- [ ] Support process defined

---

## Using GitHub Copilot Effectively

### Best Practices
1. **Clear Comments:** Write clear comments describing what you want before code
2. **Context:** Keep relevant code visible in the editor for better suggestions
3. **Iterate:** Accept suggestion, then refine with additional comments
4. **Tests First:** Use Copilot to generate test cases, then implementation
5. **Patterns:** Let Copilot handle boilerplate and repetitive patterns

### Example Prompts
```csharp
// Generate a domain entity for fixture identification with validation
// Include: PhoneNumber, ImageUrl, Manufacturer, Model, Confidence, Timestamp
// Add validation for confidence between 0.0 and 1.0

// Generate xUnit test cases for the FixtureIdentification entity
// Test: Valid entity creation, Invalid confidence value, Null phone number

// Implement Twilio service to send SMS using Twilio .NET SDK
// Include error handling and logging

// Create use case to process fixture identification
// Steps: 1. Validate input, 2. Call image recognition, 3. Check confidence
// 4. Send SMS response, 5. Create invoice if confidence > 0.8
```

---

## Risk Mitigation Strategies

### Technical Risks
1. **AI Accuracy Below Target**
   - Mitigation: Start with larger training set, iterate weekly
   - Fallback: Manual review queue for low confidence results

2. **API Rate Limits**
   - Mitigation: Implement request throttling and queuing
   - Monitoring: Track API usage proactively

3. **Cost Overruns**
   - Mitigation: Set up Azure budget alerts
   - Monitoring: Weekly cost review

### Process Risks
1. **Scope Creep**
   - Mitigation: Strict adherence to MVP feature list
   - Process: Document all "nice to have" features for post-MVP

2. **Time Delays**
   - Mitigation: Focus on critical path items first
   - Process: Daily progress review, adjust as needed

---

## Post-MVP Roadmap

### Immediate Next Steps (Post-Launch)
1. Add persistence layer (Azure Cosmos DB or SQL)
2. Implement user dashboard for query history
3. Add email notifications as alternative to SMS
4. Create admin panel for monitoring and support

### Future Enhancements
1. Mobile app for easier photo submission
2. Subscription pricing model
3. Integration with parts suppliers for instant pricing
4. Expand to other fixture types (HVAC, electrical)

---

## Success Metrics & KPIs

Track these metrics weekly:

| Metric | Target | Measurement |
|--------|--------|-------------|
| Queries per week | 50+ by week 6 | Application Insights |
| Identification accuracy | ≥80% | Custom logging |
| Average confidence | ≥0.85 | Custom logging |
| Response time | <10 sec (95th percentile) | Application Insights |
| Billing success rate | ≥95% | Custom logging |
| Cost per query | <$0.20 | Azure Cost Management |
| User retention | ≥30% repeat | Custom analytics |

---

## Contact & Support

**Technical Lead:** Harold Collins  
**Repository:** https://github.com/MasterSkriptor/PlumbingBiddingTool  
**Documentation:** /Documentation/  
**Support:** [Add support contact]

---

**Document Version History**

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | February 2025 | Development Team | Initial development plan |
