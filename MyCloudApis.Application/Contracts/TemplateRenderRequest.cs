namespace MyCloudApis.Application.Contracts;

public class TemplateRenderRequest
{
    public string Template { get; set; } = string.Empty;
    public object Model { get; set; } = new();
    public string? FileName { get; set; }
}
