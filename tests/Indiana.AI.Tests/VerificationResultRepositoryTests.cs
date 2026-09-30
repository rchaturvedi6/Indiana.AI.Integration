using System.Threading.Tasks;
using Indiana.AI.Domain.Enums;
using Indiana.AI.Domain.Models;
using Indiana.AI.Infrastructure.Persistence;

namespace Indiana.AI.Tests;

public class VerificationResultRepositoryTests
{
    [Fact]
    public async Task SaveAsync_then_GetByFileId_returns_stored_result()
    {
        var repo = new InMemoryVerificationResultRepository();
        var result = new VerificationResult
        {
            FileId = "9eb84ea3-37c0-46a1-a75b-b8935c0c63bc",
            TrackingId = "7807176902",
            Status = VerificationStatus.Pass,
            Code = "2000"
        };

        await repo.SaveAsync(result);
        var fetched = await repo.GetByFileIdAsync("9eb84ea3-37c0-46a1-a75b-b8935c0c63bc");

        Assert.NotNull(fetched);
        Assert.Equal("7807176902", fetched!.TrackingId);
        Assert.Equal(VerificationStatus.Pass, fetched.Status);
    }

    [Fact]
    public async Task GetByFileId_returns_null_when_absent()
    {
        var repo = new InMemoryVerificationResultRepository();

        Assert.Null(await repo.GetByFileIdAsync("missing"));
    }

    [Fact]
    public async Task SaveAsync_overwrites_existing_result_for_same_file()
    {
        var repo = new InMemoryVerificationResultRepository();
        await repo.SaveAsync(new VerificationResult { FileId = "F1", Status = VerificationStatus.Pass });
        await repo.SaveAsync(new VerificationResult { FileId = "F1", Status = VerificationStatus.Fail });

        var all = await repo.GetAllAsync();
        var fetched = await repo.GetByFileIdAsync("F1");

        Assert.Single(all);
        Assert.Equal(VerificationStatus.Fail, fetched!.Status);
    }
}
