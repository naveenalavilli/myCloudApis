using System.Text;
using MyCloudApis.Application.Contracts;
using MyCloudApis.Application.Interfaces;

namespace MyCloudApis.Infrastructure.Csv;

/// <summary>
/// Simple CSV import/export with configurable delimiter.
/// </summary>
public class CsvService : ICsvService
{
    public Task<byte[]> ExportAsync(CsvExportRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Rows == null || request.Rows.Count == 0)
        {
            throw new ArgumentException("At least one row is required to export CSV.", nameof(request.Rows));
        }

        var columns = request.Rows.SelectMany(r => r.Keys).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var sb = new StringBuilder();

        sb.AppendLine(string.Join(request.Delimiter, columns));

        foreach (var row in request.Rows)
        {
            var values = columns.Select(col => Escape(row.TryGetValue(col, out var val) ? val : null, request.Delimiter));
            sb.AppendLine(string.Join(request.Delimiter, values));
        }

        return Task.FromResult(Encoding.UTF8.GetBytes(sb.ToString()));
    }

    public async Task<CsvImportResult> ImportAsync(Stream csvStream, string delimiter = ",", CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(csvStream, Encoding.UTF8, leaveOpen: true);
        var content = await reader.ReadToEndAsync(cancellationToken);
        var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length == 0)
        {
            return new CsvImportResult();
        }

        var headers = lines[0].Split(delimiter);
        var result = new CsvImportResult();

        for (var i = 1; i < lines.Length; i++)
        {
            var parts = lines[i].Split(delimiter);
            var row = new Dictionary<string, string>();
            for (var j = 0; j < headers.Length; j++)
            {
                var header = headers[j];
                row[header] = j < parts.Length ? Unescape(parts[j]) : string.Empty;
            }
            result.Rows.Add(row);
        }

        csvStream.Position = 0;
        return result;
    }

    private static string Escape(object? value, string delimiter)
    {
        var str = value?.ToString() ?? string.Empty;
        var needsQuote = str.Contains(delimiter) || str.Contains("\"") || str.Contains("\n") || str.Contains("\r");
        if (needsQuote)
        {
            str = $"\"{str.Replace("\"", "\"\"")}\"";
        }
        return str;
    }

    private static string Unescape(string input)
    {
        if (input.StartsWith("\"") && input.EndsWith("\""))
        {
            var inner = input.Substring(1, input.Length - 2);
            return inner.Replace("\"\"", "\"");
        }
        return input;
    }
}
