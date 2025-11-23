namespace MyCloudApis.Application.Contracts;

public class JsonToExcelRequest
{
    public string FileName { get; set; } = "export.xlsx";
    public List<Dictionary<string, object?>> Rows { get; set; } = new();
}
