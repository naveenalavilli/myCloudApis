namespace MyCloudApis.Application.Contracts;

public class CsvImportResult
{
    public List<Dictionary<string, string>> Rows { get; set; } = new();
}
