using Microsoft.AspNetCore.Mvc;
using MyCloudApis.Application.Contracts;
using MyCloudApis.Application.Interfaces;

namespace MyCloudApis.Api.Controllers;

[ApiController]
[Route("excel")]
public class ExcelController : ControllerBase
{
    private readonly IExcelExportService _excelExportService;

    public ExcelController(IExcelExportService excelExportService)
    {
        _excelExportService = excelExportService;
    }

    [HttpPost("export")]
    public async Task<IActionResult> Export([FromBody] JsonToExcelRequest request, CancellationToken cancellationToken)
    {
        if (request == null || request.Rows == null || request.Rows.Count == 0)
        {
            return BadRequest("Provide a non-empty collection of rows to export.");
        }

        var fileName = string.IsNullOrWhiteSpace(request.FileName)
            ? $"export-{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx"
            : request.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase)
                ? request.FileName
                : $"{request.FileName}.xlsx";

        var excelBytes = await _excelExportService.ExportAsync(request, cancellationToken);
        return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}
