using MyCloudApis.Domain;

namespace MyCloudApis.Application.Contracts;

public class SendEmailRequest
{
    public EmailAddress? From { get; set; }
    public List<EmailAddress> To { get; set; } = new();
    public List<EmailAddress> Cc { get; set; } = new();
    public List<EmailAddress> Bcc { get; set; } = new();
    public string Subject { get; set; } = string.Empty;
    public string? TextBody { get; set; }
    public string? HtmlBody { get; set; }
    public List<EmailAttachment> Attachments { get; set; } = new();
    public SmtpSettings? Smtp { get; set; }
}
