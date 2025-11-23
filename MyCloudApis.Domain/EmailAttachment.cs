namespace MyCloudApis.Domain;

public class EmailAttachment
{
    public string? FilePath { get; set; }
    public string? ContentBase64 { get; set; }
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
}
