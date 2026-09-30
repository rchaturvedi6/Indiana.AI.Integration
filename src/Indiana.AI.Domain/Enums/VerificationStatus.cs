namespace Indiana.AI.Domain.Enums;

/// <summary>
/// Represents the outcome of a document verification / validation operation.
/// </summary>
public enum VerificationStatus
{
    /// <summary>Verification has not yet started or is queued.</summary>
    Pending = 0,

    /// <summary>Verification is currently being processed.</summary>
    InProgress = 1,

    /// <summary>All documents passed verification.</summary>
    Pass = 2,

    /// <summary>One or more documents failed verification.</summary>
    Fail = 3,

    /// <summary>Verification could not be completed because of an error.</summary>
    Error = 4
}
