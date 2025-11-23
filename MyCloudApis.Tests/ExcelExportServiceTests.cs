using MyCloudApis.Application.Contracts;
using MyCloudApis.Infrastructure.Excel;

namespace MyCloudApis.Tests;

public class ExcelExportServiceTests
{
    [Fact]
    public async Task ExportAsync_WritesRows()
    {
        var service = new ExcelExportService();
        var request = new JsonToExcelRequest
        {
            Rows =
            [
                new Dictionary<string, object?> { ["Id"] = 1, ["Name"] = "Alice" },
                new Dictionary<string, object?> { ["Id"] = 2, ["Name"] = "Bob" }
            ]
        };

        var result = await service.ExportAsync(request);

        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }
}
