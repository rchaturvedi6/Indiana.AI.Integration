using System.Threading;
using System.Threading.Tasks;
using Indiana.AI.Domain.Models;

namespace Indiana.AI.Application.Abstractions;

/// <summary>
/// A File Management Server (FMS / EDMS) endpoint that the AI Verification
/// Service calls back to during validation to download an uploaded file
/// (<c>GetEDMSFileQuery</c>). In this POC it is implemented by a mock that
/// returns seeded documents, so the platform can host a stand-in FMS for the AI
/// service without a real Oracle / EDMS.
/// </summary>
public interface IFileManagementService
{
    /// <summary>Retrieves an EDMS file (metadata + Base64 content) for the supplied query.</summary>
    Task<GetEdmsFileResponse> GetEdmsFileAsync(GetEdmsFileRequest request, CancellationToken cancellationToken = default);
}
