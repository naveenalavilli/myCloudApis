namespace MyCloudApis.Application.Contracts;

public class PdfFromHtmlRequest
{
    public string? HtmlContent { get; set; }
    public string? HtmlFilePath { get; set; }
    public string? FileName { get; set; }
}
