using System;
using System.Collections.Generic;
using System.Text;
using Indiana.AI.Domain.Models;

namespace Indiana.AI.Infrastructure.Adapters;

/// <summary>
/// Simulates the Provider Portal's File Management Server (EDMS) backing store.
/// Seeds a set of credentialing documents keyed by the file ids used in the AI
/// sample payloads, each represented by a minimal in-memory PDF, and exposes
/// them by file id (the mock equivalent of <c>GetEDMSFileQuery</c>, API 2).
/// </summary>
public class MockEdmsRepository
{
    private readonly Dictionary<string, EdmsQueryResult> _files;

    /// <summary>Seeds the mock EDMS store with demonstration documents.</summary>
    public MockEdmsRepository()
    {
        _files = new Dictionary<string, EdmsQueryResult>(StringComparer.OrdinalIgnoreCase);

        Seed("9eb84ea3-37c0-46a1-a75b-b8935c0c63bc", "2.Federal_W9_Tax_Form.pdf", "Federal", "Federal W-9 Form");
        Seed("8e96271d-fa17-4c57-896b-4193101aa2ca", "License.pdf", "Malprac", "License");
        Seed("4d99d591-aff0-4c55-b05e-24a0df855628", "CLIA_Certificate.pdf", "CLIA", "CLIA certificate required");
        Seed("5fe4dcb2-0553-4f8b-b5bb-6fe5683fdf4e", "Hospital_License.pdf", "HospLic", "Hospital License");
        Seed("9e4248aa-d4f6-4e04-b7ba-ad2547700538", "CMS_Certification_Letter.pdf", "CMSCertLtr", "Certification Letter from CMS");

        // A "clean" document that passes verification (no seeded extraction mismatches).
        Seed("11111111-1111-1111-1111-111111111111", "Clean_Federal_W9.pdf", "Federal", "Federal W-9 Form");
    }

    /// <summary>Returns the stored EDMS record for a file id, or <c>null</c> when absent.</summary>
    public EdmsQueryResult? GetFile(string edmsFileId) =>
        edmsFileId is not null && _files.TryGetValue(edmsFileId, out var record) ? record : null;

    private void Seed(string fileId, string fileName, string attachmentTypeCode, string attachmentTypeName)
    {
        _files[fileId] = new EdmsQueryResult
        {
            EdmsFileId = fileId,
            FileName = fileName,
            FileExtension = "pdf",
            FileData = Convert.ToBase64String(CreateMinimalPdf($"{attachmentTypeName} ({fileId})")),
            ModuleName = "Provider Enrollment",
            UserId = "PCDefaultUser",
            LastModifiedDate = DateTime.UtcNow,
            IsActive = true,
            KeyIdentifiers = new Dictionary<string, string>
            {
                ["Document Type"] = "Attachment",
                ["Document Name"] = "Provider Enrollment Attachments",
                ["Attachment Type"] = attachmentTypeCode,
                ["Attachment Type Description"] = attachmentTypeName
            }
        };
    }

    /// <summary>
    /// Builds a tiny but structurally valid single-page PDF containing the
    /// supplied title, so the Base64 payload decodes to a real PDF.
    /// </summary>
    private static byte[] CreateMinimalPdf(string title)
    {
        var content =
            "%PDF-1.4\n" +
            "1 0 obj<< /Type /Catalog /Pages 2 0 R >>endobj\n" +
            "2 0 obj<< /Type /Pages /Kids [3 0 R] /Count 1 >>endobj\n" +
            "3 0 obj<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] " +
            "/Resources << /Font << /F1 5 0 R >> >> /Contents 4 0 R >>endobj\n" +
            "4 0 obj<< /Length 68 >>stream\n" +
            $"BT /F1 18 Tf 72 720 Td ({title}) Tj ET\n" +
            "endstream endobj\n" +
            "5 0 obj<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>endobj\n" +
            "trailer<< /Root 1 0 R >>\n" +
            "%%EOF";

        return Encoding.ASCII.GetBytes(content);
    }
}
