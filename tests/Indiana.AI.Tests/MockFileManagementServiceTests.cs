using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Indiana.AI.Domain.Models;
using Indiana.AI.Infrastructure.Adapters;
using Indiana.AI.Infrastructure.Fms;

namespace Indiana.AI.Tests;

public class MockFileManagementServiceTests
{
    private static MockFileManagementService Create() =>
        new(new MockEdmsRepository());

    [Fact]
    public async Task GetEdmsFile_returns_base64_and_metadata_envelope()
    {
        var fms = Create();
        var request = new GetEdmsFileRequest
        {
            Where = new EdmsWhere { EdmsFileId = "9eb84ea3-37c0-46a1-a75b-b8935c0c63bc", TenantId = "Indiana" }
        };

        var response = await fms.GetEdmsFileAsync(request);

        Assert.Equal("9eb84ea3-37c0-46a1-a75b-b8935c0c63bc", response.QueryResult.EdmsFileId);
        Assert.Equal("2.Federal_W9_Tax_Form.pdf", response.QueryResult.FileName);
        Assert.Equal("pdf", response.QueryResult.FileExtension);
        Assert.Equal(1, response.RowCount);
        Assert.NotEmpty(response.QueryResult.KeyIdentifiers);

        var decoded = Encoding.ASCII.GetString(Convert.FromBase64String(response.QueryResult.FileData));
        Assert.StartsWith("%PDF", decoded);
    }

    [Fact]
    public async Task GetEdmsFile_throws_for_unknown_file()
    {
        var fms = Create();
        var request = new GetEdmsFileRequest
        {
            Where = new EdmsWhere { EdmsFileId = "00000000-0000-0000-0000-000000000000", TenantId = "Indiana" }
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(() => fms.GetEdmsFileAsync(request));
    }
}
