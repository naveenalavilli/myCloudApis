namespace MyCloudApis.Application.Contracts;

public class CsvExportRequest
{
    public string FileName { get; set; } = "export.csv";
    public List<Dictionary<string, object?>> Rows { get; set; } = new();
    public string Delimiter { get; set; } = ",";
}
