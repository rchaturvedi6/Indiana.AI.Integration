using System;
using System.Collections.Generic;

namespace Indiana.AI.Infrastructure.Adapters;

/// <summary>
/// Represents what the (mock) AI engine "extracts" from a given document. A
/// <c>null</c> field means the AI did not contest that value, so no error is
/// raised for it. Non-null fields are compared against the entered enrollment
/// data and produce the corresponding validation error code on mismatch.
/// </summary>
public class ExtractionProfile
{
    /// <summary>Extracted attachment type name (drives code 3001).</summary>
    public string? ExtractedAttachmentTypeName { get; set; }

    /// <summary>Extracted legal / business name (drives code 3101).</summary>
    public string? ExtractedLegalName { get; set; }

    /// <summary>Extracted address (drives code 3109).</summary>
    public string? ExtractedAddress { get; set; }

    /// <summary>Extracted CLIA number (drives code 3116).</summary>
    public string? ExtractedCliaNumber { get; set; }

    /// <summary>Extracted tax id (drives code 3120).</summary>
    public string? ExtractedTaxId { get; set; }

    /// <summary>Extracted license number (drives code 3123).</summary>
    public string? ExtractedLicenseNumber { get; set; }

    /// <summary>Extracted effective date (drives code 3127).</summary>
    public string? ExtractedEffectiveDate { get; set; }

    /// <summary>Extracted expiration / end date (drives code 3129).</summary>
    public string? ExtractedEndDate { get; set; }

    /// <summary>Extracted issue date (drives code 3150).</summary>
    public string? ExtractedIssueDate { get; set; }
}

/// <summary>
/// Seeded extraction profiles keyed by EDMS file id. Reproduces the flavour of
/// the AI sample validation response. A file id without a profile is treated as
/// a clean document that passes verification.
/// </summary>
public class MockExtractionDataset
{
    private readonly Dictionary<string, ExtractionProfile> _profiles;

    /// <summary>Seeds the extraction profiles used by the mock verifier.</summary>
    public MockExtractionDataset()
    {
        _profiles = new Dictionary<string, ExtractionProfile>(StringComparer.OrdinalIgnoreCase)
        {
            // Hospital License → AI extracts a DEA Certificate (3001).
            ["5fe4dcb2-0553-4f8b-b5bb-6fe5683fdf4e"] = new ExtractionProfile
            {
                ExtractedAttachmentTypeName = "DEA Certificate"
            },

            // CMS Certification Letter → AI extracts a Pharmacy Certificate (3001).
            ["9e4248aa-d4f6-4e04-b7ba-ad2547700538"] = new ExtractionProfile
            {
                ExtractedAttachmentTypeName = "Pharmacy Certificate"
            },

            // CLIA certificate → number, address and date mismatches.
            ["4d99d591-aff0-4c55-b05e-24a0df855628"] = new ExtractionProfile
            {
                ExtractedCliaNumber = "19D1234567",
                ExtractedAddress = "6211 Centreville Road, Centreville, Virginia 20121-2641, Fairfax County",
                ExtractedEffectiveDate = "2025-12-01",
                ExtractedEndDate = "2026-12-31"
            },

            // License → name, address and license number mismatches.
            ["8e96271d-fa17-4c57-896b-4193101aa2ca"] = new ExtractionProfile
            {
                ExtractedLegalName = "Lab Facility",
                ExtractedAddress = "6211 Centreville Road, Centreville, VA 20121-2641",
                ExtractedLicenseNumber = "8327572031"
            },

            // Federal W-9 → name, tax id and issue date mismatches.
            ["9eb84ea3-37c0-46a1-a75b-b8935c0c63bc"] = new ExtractionProfile
            {
                ExtractedLegalName = "Lab Facility",
                ExtractedTaxId = "45-2847391",
                ExtractedIssueDate = "2025-12-12"
            }
        };
    }

    /// <summary>Returns the extraction profile for a file id, or <c>null</c> when clean.</summary>
    public ExtractionProfile? GetProfile(string fileId) =>
        fileId is not null && _profiles.TryGetValue(fileId, out var profile) ? profile : null;
}
