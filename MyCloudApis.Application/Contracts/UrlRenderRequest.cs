namespace MyCloudApis.Application.Contracts;

public class UrlRenderRequest
{
    public string Url { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public bool AsImage { get; set; } = false;
}
