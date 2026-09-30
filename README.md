# Indiana AI Document Verification Integration (POC)

A .NET 8 proof-of-concept that simulates the **Indiana AI Document Verification
Integration Platform**. It implements the **real Provider Enrollment (PE) API
contracts** exchanged between the Provider Portal, the AI Verification Service
and the File Management Server (FMS), with the **AI verification engine** and
the **File Management Server** mocked so the flow can be exercised without
external dependencies or API keys.

The request/response shapes match the schema in
[`samples/API-Schema.yaml`](samples/API-Schema.yaml) and the sample payloads in
[`samples/`](samples/).

---

## 1. Purpose

- Simulate the AI Integration Platform for a live architecture demo.
- Exercise the **real contracts** (`document_upload`, `document_validate`,
  `GetEDMSFileQuery`) without standing up the AI engine or FMS.
- Provide a clean, layered codebase that maps 1:1 to the architecture and can
  later swap the mocks for the real integrations + API keys.

Implemented APIs (from the shared API list):

| # | API | This POC route | Status |
|---|-----|----------------|--------|
| 1 | Document upload request (`/pe/document_upload`) | `POST /api/document-upload` | ✅ real contract |
| 2 | FMS file download (`GetEDMSFileQuery`) | `POST /api/artifact-retrieval` | ✅ real contract (mock data) |
| 3 | Document validate request (`/pe/document_validate`) | `POST /api/document-validate` | ✅ real contract |
| 4 | Screener request (`/pe/screener/document_upload`) | — | out of scope for this POC |
| 5 | Screener disposition response | — | out of scope for this POC |

What is **mocked**:

| Component                     | Mock implementation                          |
|-------------------------------|----------------------------------------------|
| AI Document Verification Svc  | `MockDocumentVerificationService` + `MockExtractionDataset` |
| File Management Server (FMS)  | `FmsAdapter` + `MockEdmsRepository`          |
| EDMS repository               | Seeded in-memory PDFs (`GetEDMSFileQuery`)   |
| Verification result store     | `InMemoryVerificationResultRepository`       |

---

## 2. Architecture

```
Provider Portal
      │  document_upload / document_validate
      ▼
  Upload / Validate API  ─────►  AI Integration Gateway
      │                          ├── Payload Mapping   (AIPayloadMapper)
      │                          ├── Routing           (Router)
      │                          ├── Tracking IDs      (TrackingContext: trackingID + requestGUID)
      │                          └── Validation Orchestration (WorkflowService)
      ▼
AI Document Verification Service (Mock)   ◄── ValidationEngine
      │  needs the document bytes
      ▼
Artifact Retrieval Service  ──►  Router  ──►  FMS Adapter (Mock)  ──►  Mock EDMS Repository
      │
      ▼
Validation Result  ──►  Verification Result Repository (In-Memory)
```

The solution follows **Clean Architecture**. Dependencies point inward:

```
Api ─► Application ─► Domain
Api ─► Infrastructure ─► Application ─► Domain
```

Port interfaces (`IDocumentAdapter`, `IAdapterResolver`,
`IArtifactRetrievalService`, `IDocumentVerificationService`,
`IVerificationResultRepository`) live in **Application/Abstractions**; their
concrete mocks live in **Infrastructure**, so the inner layers never depend on
the outer ones.

---

## 3. Project Structure

```
Indiana.AI.Integration.sln
│
├── src
│   ├── Indiana.AI.Domain            # Models & enums matching the PE API schema
│   │   ├── Enums                    # VerificationStatus, ResponseCode, ValidationErrorCode
│   │   └── Models                   # Route, Enrollment, ProviderInformation (rich),
│   │                                #   BusinessInformation, IndividualInformation, Name,
│   │                                #   Specialty, CliaDetail, LicenseDetail, DeaDetail,
│   │                                #   PeCertification, MalpracticeInsurance, FlatAddress,
│   │                                #   AddressWrapper, Attachment, EftDetail, EducationDetail,
│   │                                #   PaymentDetail, DmeSuretyBond, BackdateRequest,
│   │                                #   UploadRequest, UploadResponse (AckResponse),
│   │                                #   DocumentValidationResponse (+ FailedDoc, ValidationError,
│   │                                #   ResultsSummary), GetEdmsFileRequest/Response (FMS),
│   │                                #   TrackingContext, AIVerificationPayload, VerificationResult
│   │
│   ├── Indiana.AI.Application       # Use cases & port interfaces
│   │   ├── Abstractions             # IAIPayloadMapper, IRouter, IValidationEngine,
│   │   │                            #   IWorkflowService, IDocumentAdapter, IAdapterResolver,
│   │   │                            #   IArtifactRetrievalService, IDocumentVerificationService,
│   │   │                            #   IVerificationResultRepository
│   │   ├── Mapping                  # AIPayloadMapper
│   │   ├── Services                 # Router, ValidationEngine, WorkflowService
│   │   └── DependencyInjection.cs   # AddApplication()
│   │
│   ├── Indiana.AI.Infrastructure    # Mock adapters, adapter framework, persistence
│   │   ├── Adapters                 # FmsAdapter, FileStorageAdapter, AdapterResolver,
│   │   │                            #   MockEdmsRepository, MockExtractionDataset,
│   │   │                            #   MockDocumentVerificationService
│   │   ├── Artifacts                # ArtifactRetrievalService
│   │   ├── Persistence              # InMemoryVerificationResultRepository
│   │   └── DependencyInjection.cs   # AddInfrastructure()
│   │
│   └── Indiana.AI.Api               # ASP.NET Core Web API
│       ├── Controllers              # DocumentsController, StatusController
│       └── Program.cs               # DI wiring, Swagger, health checks, root redirect
│
├── tests
│   └── Indiana.AI.Tests             # xUnit tests (mapping, router, artifact,
│                                    #   validation engine, repository)
│
└── samples                          # Real API schema + sample payloads (APIs 1–5)
```

---

## 4. How to Run

### Prerequisites
- .NET 8 SDK (or newer; the projects target `net8.0`).

### Build / test
```bash
dotnet build Indiana.AI.Integration.sln
dotnet test  Indiana.AI.Integration.sln
```

### Run the API
```bash
dotnet run --project src/Indiana.AI.Api
```

Open the root URL (for example `http://localhost:5192`) — it **redirects
straight to Swagger UI**.

> The API project sets `<RollForward>Major</RollForward>` so it can also run on a
> newer installed ASP.NET Core runtime if the exact 8.0 runtime is absent.

---

## 5. Swagger Endpoints

Swagger UI renders immediately at `/swagger`; the site root redirects to it. XML
comments and typed request/response models are surfaced in the UI.

| Method | Route                                   | Description                                      |
|--------|-----------------------------------------|--------------------------------------------------|
| POST   | `/api/document-upload`                  | API 1 — accepts `route` + `enrollment`, echoes `route` + `ack_status` (1002/PASS). |
| POST   | `/api/document-validate`                | API 3 — validates, persists results, returns `route` + `failedDocs` + `resultsSummary`. |
| POST   | `/api/artifact-retrieval`               | API 2 — `GetEDMSFileQuery` envelope, returns Base64 file content. |
| GET    | `/api/verification-results/{fileId}`    | Returns the stored verification result for a file. |
| GET    | `/api/status`                           | Gateway heartbeat.                               |
| GET    | `/health`                               | Health check.                                    |

Full example payloads are in [`src/Indiana.AI.Api/Indiana.AI.Api.http`](src/Indiana.AI.Api/Indiana.AI.Api.http)
and [`samples/`](samples/).

### API 1 — upload (abbreviated)
```json
POST /api/document-upload
{
  "route": { "domain": "prov-enroll", "requestGUID": "2965796994", "requestType": "document_upload", "tenantID": "…", "appClient": "prov_portal_ftest" },
  "enrollment": {
    "trackingID": "7807176902",
    "providerInformation": {
      "npi": "1269201798",
      "businessInformation": { "legalName": "Inuvik Regional Hospital Research" },
      "attachments": [ { "fileId": "5fe4dcb2-0553-4f8b-b5bb-6fe5683fdf4e", "fileName": "Hospital_License.pdf", "attachmentTypeCode": "HospLic", "attachmentTypeName": "Hospital License" } ]
    }
  }
}
```
Response echoes `route` and adds:
```json
"ack_status": { "code": "1002", "status": "PASS", "processingTimeMs": 1.8, "timestamp": "…" }
```

### API 3 — validate (abbreviated)
```json
{
  "route": { "requestGUID": "2965796994", "requestType": "document_validate" },
  "enrollment": { "trackingID": "7807176902", "providerInformation": { "attachments": [ … ] } }
}
```
Response (mismatches produce the real error codes):
```json
{
  "route": { … },
  "failedDocs": [
    { "fileId": "5fe4dcb2-0553-4f8b-b5bb-6fe5683fdf4e", "validation_errors": [ { "code": "3001", "entered": "HospLic", "extracted": "DEA Certificate" } ] }
  ],
  "resultsSummary": { "code": "2001", "status": "FAIL", "totalDocs": 6, "passedDocs": 0, "failedDocs": 5, "errorDocs": 0 }
}
```

### API 2 — FMS retrieval
```json
POST /api/artifact-retrieval
{ "where": { "edmsFileId": "9eb84ea3-37c0-46a1-a75b-b8935c0c63bc", "moduleName": "Provider Enrollment" } }
```
```json
{ "where": { … }, "queryResult": { "edmsFileId": "…", "fileName": "2.Federal_W9_Tax_Form.pdf", "fileExtension": "pdf", "fileData": "JVBERi0xLjQ…", "keyIdentifiers": { … } }, "rowCount": 1 }
```

**Seeded EDMS file ids** (from the AI sample):
`9eb84ea3-…` Federal W-9, `8e96271d-…` License, `4d99d591-…` CLIA,
`5fe4dcb2-…` Hospital License, `9e4248aa-…` CMS Certification Letter, and a
"clean" `11111111-1111-1111-1111-111111111111` that passes.

### Codes

Acknowledgement / summary:

| Code | Meaning              |
|------|----------------------|
| 1002 | Upload accepted      |
| 2000 | Validation passed    |
| 2001 | Validation failed    |
| 4004 | File / result not found |

Field-level `validation_errors` (`ValidationErrorCode`):

| Code | Meaning                       |
|------|-------------------------------|
| 3001 | attachment type mismatch      |
| 3101 | name / legal name mismatch    |
| 3109 | address mismatch              |
| 3116 | CLIA number mismatch          |
| 3120 | tax id mismatch               |
| 3123 | license number mismatch       |
| 3127 | effective date mismatch       |
| 3129 | expiration / end date mismatch|
| 3133 | generic field mismatch        |
| 3150 | issue date mismatch           |

The mock verifier drives these codes from seeded per-document extraction
profiles (`MockExtractionDataset`), comparing them against the entered
enrollment values, so validating the provided sample payload reproduces the AI
sample response (5 failed docs across 6 attachments).

---

## 6. Switching between mock and live (Enso / FMS)

Every mock sits behind an Application port interface, so flipping the whole
solution between **mock** and **live** is a single configuration flag — no code
changes to the Domain, Application or API layers.

### Configuration

`src/Indiana.AI.Api/appsettings.json`:

```jsonc
"Integration": {
  "UseMocks": true,                       // false = call the live Enso + FMS APIs
  "Enso": {
    "BaseUrl": "https://enso-apis.enso.dev.pgwcloud.net",
    "UploadPath": "/pe/document_upload",
    "ValidatePath": "/pe/document_validate",
    "ApiKey": "",                          // supply the issued X-API-Key
    "Source": "provider_portal"            // sent as X-Source
  },
  "Fms": {
    "BaseUrl": "https://bas-fltest-perf.mapshc.com/HPP.Core.FileMgmtApi",
    "GetEdmsFilePath": "/R6.0/FileMgmtQuery/GetEDMSFileQuery"
  }
}
```

- `UseMocks: true` (default) → in-process mocks (`LocalAiPlatformClient`,
  `FmsAdapter`, `MockDocumentVerificationService`, seeded EDMS data).
- `UseMocks: false` → live typed `HttpClient`s: `EnsoAiPlatformClient`
  (upload + validate, with `X-API-Key` / `X-Source` headers) and `RealFmsAdapter`
  (`GetEDMSFileQuery`).

### How the swap works

The seam is the `IAiPlatformClient` interface (upload + validate):

```csharp
// Application (default): in-process simulation
services.AddScoped<IAiPlatformClient, LocalAiPlatformClient>();

// Infrastructure (when UseMocks == false): replaced by the live Enso client
services.AddScoped<IAiPlatformClient>(sp => new EnsoAiPlatformClient(http, uploadPath, validatePath));
```

`AddInfrastructure(configuration)` reads the flag and calls either
`AddMockIntegration()` or `AddLiveIntegration(options)`. Verification-result
persistence stays local in both modes. The two toggles are independent, so you
can also run a hybrid (for example live FMS + mock AI) by extending the
registration.

### Going further

1. **Durable persistence** — implement `IVerificationResultRepository` over a
   database (EF Core / Dapper) in place of the in-memory store.
2. **Screener APIs (4 & 5)** — add a screener upload endpoint and the Appian
   `peApplicationReviewResponse` callback when that flow is in scope.

> **Onboarding note:** calling the live AI Verification Service (APIs 1, 3, 4)
> requires an issued API key (`X-API-Key`) and source identifier (`X-Source`).
> Set `Integration:Enso:ApiKey` and flip `UseMocks` to `false` once onboarded.

---

## 7. Tests

`dotnet test` runs the xUnit suite covering:

- **Payload mapping** — tracking ids derived from `route.requestGUID` /
  `enrollment.trackingID`, and enrollment→AI projection.
- **Router resolution** — module→channel mapping, defaults, supported channels.
- **Artifact retrieval** — `GetEDMSFileQuery` envelope, Base64 PDF, not-found.
- **Validation engine** — PASS on clean doc; FAIL with `3001` and the CLIA
  field-mismatch codes; multi-document counts.
- **Repository** — save / get / overwrite semantics.
