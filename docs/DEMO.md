# POC Demo Cheat-Sheet — Indiana AI Document Verification

## 30-second pitch
This POC is the **AI Integration Gateway** — the middleware between Indiana's
InterChange portal and the AI Document Verification Service. It takes an
enrollment, pulls each uploaded document from the state's store, has the AI check
that what the provider *typed* matches what the documents *say*, and returns
pass/fail per document. **The AI engine and the document store are mocked**, but
every contract, the tenant routing, and the orchestration are real.

## Mental model
```
InterChange/Portal ──►  AI Gateway (this POC)  ──►  AI Verification Service (mock)
                              │                            │ needs the file
                              │                            ▼
                              └── tenant routing ──►  Oracle/FMS store (mock)
                                                            │
                                     persist result ◄───────┘
```

## Run it (before the meeting)
```powershell
cd <repo root>\Indiana.AI.Integration
dotnet run --project src/Indiana.AI.Api
```
Browser opens / root redirects to **Swagger** at `/swagger`. Demo from Swagger.

## Demo flow (5 steps in Swagger)
1. **Open Swagger** — "The 5 gateway endpoints InterChange calls."
2. **POST /api/document-upload** — paste `samples/1 - Doc Upload Request.json`
   → point at `"code": "1002"`. *Fast acknowledgement; portal isn't blocked.*
3. **POST /api/document-validate** — paste `samples/3 - Doc Validate Request.json`
   → scroll `failedDocs`. *AI compared entered vs extracted: hospital license is
   actually a DEA cert (3001), CLIA mismatch (3116), address mismatch (3109).*
4. **POST /api/artifact-retrieval** — body:
   `{ "where": { "edmsFileId": "9eb84ea3-37c0-46a1-a75b-b8935c0c63bc", "tenantId": "Indiana" } }`
   → point at `fileData` (Base64) + `keyIdentifiers`. *Indiana auto-routed to Oracle adapter.*
5. **GET /api/verification-results/5fe4dcb2-0553-4f8b-b5bb-6fe5683fdf4e**
   → *Result was persisted; portal can read it back anytime.*

## Upload vs Validate (the key concept)
| | document-upload | document-validate |
|---|---|---|
| Purpose | Intake + ack | Verify documents |
| Downloads files / runs AI / persists | No | Yes |
| Response | `ack_status` 1002 | `failedDocs` + `resultsSummary` 2000/2001 |
Same payload, two phases: quick receipt, then the (re-runnable) AI verification.

## 3 architecture wins to name-drop
- **Tenant-aware routing** — Indiana→Oracle, Nevada→SQL, FutureState→File, from
  `appsettings.json`. Add a state = one config line, zero code.
- **Mock/live toggle** — `"Integration:UseMocks": true`. Flip to false + drop in
  the API key = real Enso + FMS. No rewrite.
- **Clean Architecture** — AI engine, document store, and DB are all swappable
  behind interfaces.

## Validation error codes (say a couple, don't read all)
3001 attachment-type · 3101 name · 3109 address · 3116 CLIA · 3120 tax id ·
3123 license # · 3127 effective date · 3129 expiration · 3150 issue date.

## If asked "what's not done?"
- **APIs 4 & 5 (Screener flow)** — out of scope for this POC.
- **OPEN questions for your team:** exact upload/validate triggers; the real FMS
  `GetEDMSFileQuery` envelope (sample files look label-swapped); live Enso/FMS
  endpoints + API keys.

## Fallback (if Swagger/live fails)
Open `src/Indiana.AI.Api/Indiana.AI.Api.http` (has all requests) or show
`samples/3 - Doc Validate Response.json` as the expected output.

---

# Ready-to-paste payloads (copy straight into Swagger "Try it out")

All use seeded file IDs and `tenantID: Indiana`, so they work out of the box.

## A) POST /api/document-upload  → expect ack 1002 / PASS
```json
{
  "route": { "domain": "prov-enroll", "apiVer": 1.5, "tenantID": "Indiana", "appClient": "interchange", "clientID": "", "requestGUID": "REQ-1001", "requestType": "document_upload", "timeStampId": "260924100000" },
  "enrollment": {
    "trackingID": "TRK-5001",
    "applicationType": "NewEnrollment",
    "enrollmentType": "Facility",
    "providerType": "Hospital",
    "providerInformation": {
      "npi": "1269201798",
      "ssn": "733172867",
      "businessInformation": { "legalName": "Inuvik Regional Hospital Research" },
      "attachments": [
        { "fileId": "5fe4dcb2-0553-4f8b-b5bb-6fe5683fdf4e", "fileName": "Hospital_License.pdf", "attachmentTypeCode": "HospLic", "attachmentTypeName": "Hospital License" }
      ]
    }
  }
}
```

## B) POST /api/document-validate  → expect 2001 / FAIL (5 failed docs)
```json
{
  "route": { "domain": "prov-enroll", "apiVer": 1.5, "tenantID": "Indiana", "appClient": "interchange", "clientID": "", "requestGUID": "REQ-1001", "requestType": "document_validate", "timeStampId": "260924100000" },
  "enrollment": {
    "trackingID": "TRK-5001",
    "applicationType": "NewEnrollment",
    "enrollmentType": "Facility",
    "providerType": "Hospital",
    "providerInformation": {
      "npi": "1269201798",
      "ssn": "733172867",
      "businessInformation": { "legalName": "Inuvik Regional Hospital Research" },
      "cliaDetails": [ { "cliaNumber": "15B3453646", "effectiveDate": "2026-07-17T00:00:00", "endDate": "9999-12-31T00:00:00" } ],
      "licenseDetails": [ { "licenseNumber": "23D6533475", "effectiveDate": "2023-01-01T00:00:00", "expirationDate": "2029-12-31T00:00:00" } ],
      "addresses": [ { "isPrimaryLocation": true, "address": { "line1": "4548 GOOD ADAMS LN", "city": "VIRGINIA BEACH", "stateId": "VA", "postalCode": "234554940" } } ],
      "backdateRequest": { "issueDate": "2026-09-18T00:00:00" },
      "attachments": [
        { "fileId": "9eb84ea3-37c0-46a1-a75b-b8935c0c63bc", "fileName": "W9.pdf", "attachmentTypeCode": "Federal", "attachmentTypeName": "Federal W-9 Form" },
        { "fileId": "8e96271d-fa17-4c57-896b-4193101aa2ca", "fileName": "License.pdf", "attachmentTypeCode": "Malprac", "attachmentTypeName": "License" },
        { "fileId": "4d99d591-aff0-4c55-b05e-24a0df855628", "fileName": "CLIA.pdf", "attachmentTypeCode": "CLIA", "attachmentTypeName": "CLIA certificate required" },
        { "fileId": "5fe4dcb2-0553-4f8b-b5bb-6fe5683fdf4e", "fileName": "Hospital_License.pdf", "attachmentTypeCode": "HospLic", "attachmentTypeName": "Hospital License" },
        { "fileId": "9e4248aa-d4f6-4e04-b7ba-ad2547700538", "fileName": "CMS_Cert.pdf", "attachmentTypeCode": "CMSCertLtr", "attachmentTypeName": "Certification Letter from CMS" }
      ]
    }
  }
}
```

## C) POST /api/document-validate  → expect 2000 / PASS (clean doc)
```json
{
  "route": { "tenantID": "Indiana", "requestGUID": "REQ-2002", "requestType": "document_validate" },
  "enrollment": {
    "trackingID": "TRK-5002",
    "providerInformation": {
      "npi": "1269201798",
      "businessInformation": { "legalName": "Inuvik Regional Hospital Research" },
      "attachments": [
        { "fileId": "11111111-1111-1111-1111-111111111111", "fileName": "Clean_W9.pdf", "attachmentTypeCode": "Federal", "attachmentTypeName": "Federal W-9 Form" }
      ]
    }
  }
}
```

## D) POST /api/artifact-retrieval  → Base64 PDF + metadata
```json
{ "where": { "edmsFileId": "9eb84ea3-37c0-46a1-a75b-b8935c0c63bc", "tenantId": "Indiana", "moduleName": "Provider Enrollment" } }
```
Try `"tenantId": "Nevada"` or `"FutureState"` to show tenant routing to a different adapter.

## E) GET /api/verification-results/{fileId}  → stored result
Run **B** first, then use this fileId in the path:
```
5fe4dcb2-0553-4f8b-b5bb-6fe5683fdf4e
```

## Seeded file IDs
| fileId | Document | Validate outcome |
|--------|----------|------------------|
| `9eb84ea3-37c0-46a1-a75b-b8935c0c63bc` | W-9 | fails (name / tax / issue date) |
| `8e96271d-fa17-4c57-896b-4193101aa2ca` | License | fails (name / address / license #) |
| `4d99d591-aff0-4c55-b05e-24a0df855628` | CLIA | fails (CLIA # / address / dates) |
| `5fe4dcb2-0553-4f8b-b5bb-6fe5683fdf4e` | Hospital License | fails (3001 → DEA Certificate) |
| `9e4248aa-d4f6-4e04-b7ba-ad2547700538` | CMS Cert Letter | fails (3001 → Pharmacy Certificate) |
| `11111111-1111-1111-1111-111111111111` | Clean W-9 | **passes (2000)** |

> Only `attachments[].fileId` must match a seeded ID; every other value can be any realistic text.
> For a live-Enso demo, use the **`https (Live Enso)`** run profile and set the key via User Secrets (`Integration:Enso:ApiKey`).
