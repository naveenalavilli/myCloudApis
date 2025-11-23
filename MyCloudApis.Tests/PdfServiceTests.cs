using MyCloudApis.Infrastructure.Pdf;

namespace MyCloudApis.Tests;

public class PdfServiceTests
{
    [Fact]
    public async Task FromHtmlAsync_ReturnsBytes()
    {
        var service = new PdfService();
        var html = "<h1>Test</h1><p>Sample content.</p>";

        var result = await service.FromHtmlAsync(html);

        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }
}
