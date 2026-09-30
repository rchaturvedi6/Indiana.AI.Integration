using System.Collections.Generic;
using Indiana.AI.Domain.Models;

namespace Indiana.AI.Application.Abstractions;

/// <summary>
/// The mocked AI Document Verification Service. Given a claimed attachment, the
/// entered provider information and the retrieved artifact, it produces the
/// field-level <see cref="ValidationError"/>s (if any) for that document.
/// </summary>
public interface IDocumentVerificationService
{
    /// <summary>
    /// Verifies a single attachment against the entered enrollment data and the
    /// retrieved artifact. Returns an empty list when the document is valid.
    /// </summary>
    List<ValidationError> Verify(Attachment attachment, ProviderInformation provider, EdmsQueryResult artifact);
}
