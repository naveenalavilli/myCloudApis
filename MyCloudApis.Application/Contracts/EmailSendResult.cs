namespace MyCloudApis.Application.Contracts;

public class EmailSendResult
{
    public bool Accepted { get; set; }
    public string? MessageId { get; set; }
    public string? Error { get; set; }
}
