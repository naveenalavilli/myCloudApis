using Microsoft.AspNetCore.Mvc;
using MyCloudApis.Application.Contracts;
using MyCloudApis.Application.Interfaces;

namespace MyCloudApis.Api.Controllers;

[ApiController]
[Route("csv")]
public class CsvController : ControllerBase
{
    private readonly ICsvService _csvService;

    public CsvController(ICsvService csvService)
    {
        _csvService = csvService;
    }

    [HttpPost("export")]
    public async Task<IActionResult> Export([FromBody] CsvExportRequest request, CancellationToken cancellationToken)
    {
        if (request == null || request.Rows == null || request.Rows.Count == 0)
        {
            return BadRequest("Provide rows to export.");
        }

        var fileName = string.IsNullOrWhiteSpace(request.FileName)
            ? $"export-{DateTime.UtcNow:yyyyMMddHHmmss}.csv"
            : request.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase)
                ? request.FileName
                : $"{request.FileName}.csv";

        var bytes = await _csvService.ExportAsync(request, cancellationToken);
        return File(bytes, "text/csv", fileName);
    }

    [HttpPost("import")]
    public async Task<IActionResult> Import([FromForm] IFormFile file, [FromForm] string delimiter = ",", CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Upload a CSV file via form-data.");
        }

        await using var stream = file.OpenReadStream();
        var result = await _csvService.ImportAsync(stream, delimiter, cancellationToken);
        return Ok(result);
    }
}
