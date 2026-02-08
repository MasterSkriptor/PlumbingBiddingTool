# Product Requirements Document

**Product Name:** Plumbing Fixture Identifier (PFI Service)  
**Version:** MVP 1.0  
**Date:** February 2025  
**Owner:** Harold Collins  
**Status:** In Development

---

## 1. One-Sentence Overview

A serverless SMS-based service that lets plumbers text a photo of a plumbing fixture (faucet, toilet, etc.) → instantly identifies the manufacturer and model using AI → replies with the answer and bills the user via QuickBooks Online.

---

## 2. Problem & Opportunity

### Problem
Plumbers waste time manually looking up fixture brands/models/parts when on-site or quoting jobs. Existing lookup tools are slow, require apps/websites, or lack accuracy for visual identification.

### Opportunity
High-accuracy AI image recognition + instant SMS delivery + automated billing creates a fast, frictionless tool that saves time and generates recurring revenue.

---

## 3. Target Users

### Primary Users
- Independent plumbers
- Small plumbing companies in the US (starting in Mississippi)

### Pain Points
- Need quick part identification without leaving the job site
- Prefer text/SMS over downloading apps

### Future Expansion
- Contractors
- Suppliers
- DIY users via email/mobile

---

## 4. Core Value Proposition

> **"Text a photo → Get manufacturer + model in seconds → Pay only for successful answers — all via your existing QuickBooks."**

---

## 5. MVP Goals & Success Metrics

### Business Goals
- Validate demand with 50–100 real queries in first month
- Achieve 80–90% identification accuracy on good photos
- Generate first revenue via QuickBooks invoices

### Key Metrics

| Metric | Target | Description |
|--------|--------|-------------|
| Identification success rate | ≥80% | Confidence threshold >0.8 |
| End-to-end latency | <10 seconds | MMS receive → SMS reply |
| Billing success | 95% | Successful QBO invoice creation |
| Cost per query | <$0.20 | Twilio + Azure AI + QuickBooks fees |
| User retention | ≥30% | Repeat users in first month |

---

## 6. MVP Features & Scope

### Must-Have Features

| # | Feature | Description | Priority | Implementation Notes |
|---|---------|-------------|----------|---------------------|
| 1 | MMS Image Reception | Receive photo via Twilio MMS webhook | Must | Phone number purchased; webhook to Azure Function |
| 2 | AI Fixture Identification | Use Azure Custom Vision to detect manufacturer + model | Must | Train on 200–500 labeled images; threshold 0.8 confidence |
| 3 | SMS Response | Reply with result or "try clearer photo" | Must | Keep <160 chars; friendly tone |
| 4 | Per-Query Billing | Create/find QBO customer by phone → generate $0.75 invoice | Must | Use Intuit .NET SDK; sandbox first |
| 5 | Clean Architecture | Domain / Application / Infrastructure / Functions layers | Must | Enables future email/mobile |
| 6 | Basic Error Handling | Low confidence → polite retry message; token refresh on 401 | Should | Log failures |
| 7 | Local + Azure Deployment | Run locally + deploy to Azure Functions | Must | CI/CD via Azure DevOps |
| 8 | Domain Mapping | api.excels.com points to Azure endpoint | Should | Network Solutions DNS |

### Out of Scope for MVP

- User portal / dashboard
- Subscriptions (only per-use billing)
- Email invoice delivery
- Mobile app submission
- Refunds / credits system
- Multi-language support
- Advanced analytics / reporting

---

## 7. High-Level Technical Architecture

### Tech Stack
- **Backend:** .NET 8, Azure Functions (serverless)
- **SMS/MMS:** Twilio API
- **AI:** Azure Custom Vision
- **Billing:** QuickBooks Online API
- **DevOps:** GitHub, Azure DevOps
- **Deployment:** Azure Functions with HTTPS

### Non-Functional Requirements

#### Security
- HTTPS only for all communications
- Secrets in local.settings.json → migrate to Azure Key Vault
- OAuth 2.0 for QuickBooks integration

#### Performance
- Scale to 1,000 queries/month with low cost
- Sub-10 second response time
- Efficient image processing

#### Reliability
- Idempotent operations
- Retry logic for transient errors
- Graceful degradation on service failures

#### Cost Target
- <$50/month at low volume
- Pay-per-use model for scalability

---

## 8. System Architecture

### Architecture Layers

```
┌─────────────────────────────────────┐
│     Azure Functions (HTTP)          │
│  - Twilio MMS Webhook Handler       │
└─────────────────┬───────────────────┘
                  │
┌─────────────────▼───────────────────┐
│      Application Layer              │
│  - Use Cases                        │
│  - Business Logic Orchestration     │
└─────────────────┬───────────────────┘
                  │
┌─────────────────▼───────────────────┐
│        Domain Layer                 │
│  - Entities (Fixture, Invoice)      │
│  - Value Objects                    │
│  - Domain Services                  │
└─────────────────────────────────────┘
                  │
┌─────────────────▼───────────────────┐
│    Infrastructure Layer             │
│  - Twilio Service                   │
│  - Azure Custom Vision Service      │
│  - QuickBooks Service               │
└─────────────────────────────────────┘
```

### Data Flow

1. **User sends MMS** → Twilio receives photo + phone number
2. **Twilio webhook** → Azure Function receives request
3. **Image Analysis** → Azure Custom Vision identifies fixture
4. **Response Generation** → Format manufacturer + model
5. **SMS Reply** → Send result to user via Twilio
6. **Billing** → Create/update customer in QuickBooks → Generate invoice

---

## 9. Timeline & Milestones

### Rough 4–6 Week Timeline from Start

| Week | Milestone | Key Deliverables |
|------|-----------|------------------|
| Week 1 | Project Setup & Foundation | Clean architecture structure, Twilio basics, local dev environment |
| Week 2 | AI Integration | Azure Custom Vision training + integration, image processing |
| Week 3 | Billing Integration | QuickBooks OAuth setup, customer management, invoice generation |
| Week 4 | Testing & Deployment | End-to-end testing, Azure deployment, DNS configuration |
| Week 5–6 | Beta Testing & Iteration | Real-user testing, accuracy improvements, polish |

---

## 10. Risks & Mitigations

| Risk | Impact | Mitigation Strategy |
|------|--------|-------------------|
| AI accuracy too low | High | Start with 50 images per common model; iterate weekly with real failures |
| OAuth / QuickBooks approval delays | Medium | Build/test fully in sandbox environment first |
| Twilio MMS costs | Medium | Monitor closely; consider offering first 3 queries free |
| User adoption | High | Target local Mississippi plumbers via networks/forums |
| Image quality issues | Medium | Provide clear guidelines; implement preprocessing |
| API rate limits | Low | Implement proper throttling and caching |

---

## 11. User Experience Flow

### Happy Path

1. User texts photo to dedicated phone number
2. System receives MMS and processes image
3. AI identifies fixture with high confidence (>0.8)
4. User receives SMS: "Identified: Kohler Wellworth K-3987. Invoice sent to your QuickBooks."
5. QuickBooks invoice created for $0.75

### Low Confidence Path

1. User texts photo to dedicated phone number
2. System receives MMS and processes image
3. AI returns low confidence (<0.8)
4. User receives SMS: "Unable to identify. Please send a clearer, closer photo of the manufacturer label or model number."

### Error Path

1. User texts photo to dedicated phone number
2. System encounters error (service down, bad image format)
3. User receives SMS: "Sorry, we encountered an error. Please try again or contact support."

---

## 12. Next Actions

### Immediate Priority (Week 1)
1. ✅ Set up clean architecture project structure
2. Finalize Azure Custom Vision dataset (collect/upload images)
3. Implement Twilio MMS webhook handler
4. Set up development environment with local.settings.json

### Week 2-3
5. Implement & test QuickBooks billing in sandbox
6. Integrate Azure Custom Vision API
7. Build end-to-end test scenarios

### Week 4+
8. Deploy MVP to Azure
9. Configure api.excels.com DNS
10. Run first real MMS tests
11. Track metrics in Azure Application Insights

---

## 13. Success Criteria for MVP Launch

- [ ] Successfully receive and process MMS images via Twilio
- [ ] AI identification accuracy ≥80% on test dataset
- [ ] Response time <10 seconds for 95% of requests
- [ ] QuickBooks invoices generated automatically
- [ ] Successfully process 10 test queries end-to-end
- [ ] Deploy to Azure with custom domain
- [ ] Documentation complete for setup and usage
- [ ] Cost per query <$0.20 validated

---

## 14. Post-MVP Roadmap (Future Considerations)

### Phase 2 (3-6 months)
- User dashboard for query history
- Email notification option
- Batch processing capability
- Enhanced reporting

### Phase 3 (6-12 months)
- Mobile app for easier photo submission
- Subscription plans
- Multi-language support
- Integration with parts suppliers

### Phase 4 (12+ months)
- Expand to other trades (HVAC, electrical)
- API for third-party integrations
- Advanced analytics dashboard
- White-label solution for suppliers

---

## Appendix A: Glossary

- **MMS:** Multimedia Messaging Service - SMS with image/media capability
- **QBO:** QuickBooks Online
- **Azure Custom Vision:** Microsoft's custom image classification service
- **Clean Architecture:** Software design pattern separating concerns into layers
- **Serverless:** Cloud computing model where provider manages infrastructure

---

## Appendix B: References

- Twilio MMS Documentation: https://www.twilio.com/docs/sms/tutorials/how-to-receive-and-reply
- Azure Custom Vision: https://azure.microsoft.com/services/cognitive-services/custom-vision-service/
- QuickBooks Online API: https://developer.intuit.com/app/developer/qbo/docs/get-started
- Clean Architecture: https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html

---

**Document Version History**

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | February 2025 | Harold Collins | Initial MVP specification |
