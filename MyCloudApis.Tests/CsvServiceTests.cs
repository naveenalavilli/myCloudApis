using System.Text;
using MyCloudApis.Application.Contracts;
using MyCloudApis.Infrastructure.Csv;

namespace MyCloudApis.Tests;

public class CsvServiceTests
{
    [Fact]
    public async Task ExportImport_RoundTrip_Works()
    {
        var service = new CsvService();
        var export = new CsvExportRequest
        {
            Delimiter = ",",
            Rows =
            [
                new Dictionary<string, object?> { ["Id"] = 1, ["Name"] = "Ada" },
                new Dictionary<string, object?> { ["Id"] = 2, ["Name"] = "Alan" }
            ]
        };

        var bytes = await service.ExportAsync(export);
        await using var stream = new MemoryStream(bytes);
        var result = await service.ImportAsync(stream);

        Assert.Equal(2, result.Rows.Count);
        Assert.Equal("Ada", result.Rows[0]["Name"]);
    }
}
