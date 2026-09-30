using System.Collections.Generic;
using Indiana.AI.Domain.Models;

namespace Indiana.AI.Tests;

/// <summary>Shared builders for constructing test upload payloads.</summary>
internal static class TestData
{
    public const string CleanFileId = "11111111-1111-1111-1111-111111111111";
    public const string HospLicFileId = "5fe4dcb2-0553-4f8b-b5bb-6fe5683fdf4e";
    public const string CliaFileId = "4d99d591-aff0-4c55-b05e-24a0df855628";

    public static UploadRequest Upload(params Attachment[] attachments)
    {
        var request = new UploadRequest
        {
            Route = new Route
            {
                Domain = "prov-enroll",
                TenantID = "b37b1cec-6603-2d4f-ac37-74e0724774a1",
                AppClient = "prov_portal_ftest",
                RequestGUID = "2965796994",
                RequestType = "document_upload"
            },
            Enrollment = new Enrollment
            {
                TrackingID = "7807176902",
                ApplicationType = "NewEnrollment",
                EnrollmentType = "Facility",
                ProviderType = "Hospital",
                ProviderInformation = new ProviderInformation
                {
                    Npi = "1269201798",
                    Ssn = "733172867",
                    BusinessInformation = new BusinessInformation { LegalName = "Inuvik Regional Hospital Research" },
                    CliaDetails = new List<CliaDetail>
                    {
                        new() { CliaNumber = "15B3453646", EffectiveDate = "2026-07-17T00:00:00", EndDate = "9999-12-31T00:00:00" }
                    },
                    LicenseDetails = new List<LicenseDetail>
                    {
                        new() { LicenseNumber = "23D6533475", ExpirationDate = "2029-12-31T00:00:00", EffectiveDate = "2023-01-01T00:00:00" }
                    },
                    Addresses = new List<AddressWrapper>
                    {
                        new()
                        {
                            IsPrimaryLocation = true,
                            Address = new FlatAddress
                            {
                                Line1 = "4548 GOOD ADAMS LN", City = "VIRGINIA BEACH",
                                StateId = "VA", PostalCode = "234554940"
                            }
                        }
                    },
                    BackdateRequest = new BackdateRequest { IssueDate = "2026-09-18T00:00:00" }
                }
            }
        };

        request.Enrollment.ProviderInformation.Attachments.AddRange(attachments);
        return request;
    }

    public static Attachment Attachment(string fileId, string typeCode, string typeName, string fileName = "doc.pdf") =>
        new() { FileId = fileId, FileName = fileName, AttachmentTypeCode = typeCode, AttachmentTypeName = typeName };
}
