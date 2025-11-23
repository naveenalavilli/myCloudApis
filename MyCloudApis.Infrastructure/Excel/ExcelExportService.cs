using ClosedXML.Excel;
using MyCloudApis.Application.Contracts;
using MyCloudApis.Application.Interfaces;

namespace MyCloudApis.Infrastructure.Excel;

/// <summary>
/// Converts flat JSON dictionaries into an Excel worksheet using ClosedXML.
/// </summary>
public class ExcelExportService : IExcelExportService
{
    public Task<byte[]> ExportAsync(JsonToExcelRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Rows == null || request.Rows.Count == 0)
        {
            throw new ArgumentException("At least one row of data is required to export Excel.", nameof(request.Rows));
        }

        var columns = request.Rows
            .SelectMany(r => r.Keys)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.AddWorksheet("Data");

        for (var i = 0; i < columns.Count; i++)
        {
            worksheet.Cell(1, i + 1).Value = columns[i];
            worksheet.Cell(1, i + 1).Style.Font.Bold = true;
        }

        for (var rowIndex = 0; rowIndex < request.Rows.Count; rowIndex++)
        {
            var row = request.Rows[rowIndex];
            for (var colIndex = 0; colIndex < columns.Count; colIndex++)
            {
                var key = columns[colIndex];
                row.TryGetValue(key, out var value);
                var cell = worksheet.Cell(rowIndex + 2, colIndex + 1);
                cell.SetValue(value?.ToString() ?? string.Empty);
            }
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return Task.FromResult(stream.ToArray());
    }
}
