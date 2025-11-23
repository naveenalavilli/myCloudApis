using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MyCloudApis.Application.Contracts;
using MyCloudApis.Application.Interfaces;
using MyCloudApis.Domain;

namespace MyCloudApis.Infrastructure.Email;

/// <summary>
/// Sends email via SMTP using MailKit with per-request overrides and optional attachments.
/// </summary>
public class EmailService : IEmailService
{
    private readonly EmailOptions _options;

    public EmailService(IOptions<EmailOptions> options)
    {
        _options = options.Value ?? new EmailOptions();
    }

    public async Task<EmailSendResult> SendAsync(SendEmailRequest request, CancellationToken cancellationToken = default)
    {
        if (!request.To.Any() && !request.Cc.Any() && !request.Bcc.Any())
        {
            return new EmailSendResult { Error = "At least one recipient is required." };
        }

        var smtp = request.Smtp ?? _options.DefaultSmtp;
        if (smtp == null || string.IsNullOrWhiteSpace(smtp.Host))
        {
            return new EmailSendResult { Error = "SMTP settings are missing. Provide them in the request or configuration." };
        }

        var fromAddress = request.From ?? (!string.IsNullOrWhiteSpace(smtp.FromAddress)
            ? new EmailAddress { Address = smtp.FromAddress, Name = smtp.FromName }
            : null);

        if (fromAddress == null || string.IsNullOrWhiteSpace(fromAddress.Address))
        {
            return new EmailSendResult { Error = "From address is required. Provide it in the request or default SMTP settings." };
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromAddress.Name ?? string.Empty, fromAddress.Address));
        AddRecipients(message.To, request.To);
        AddRecipients(message.Cc, request.Cc);
        AddRecipients(message.Bcc, request.Bcc);

        message.Subject = string.IsNullOrWhiteSpace(request.Subject) ? "(no subject)" : request.Subject;

        var builder = new BodyBuilder
        {
            HtmlBody = request.HtmlBody,
            TextBody = request.TextBody
        };

        foreach (var attachment in request.Attachments)
        {
            TryAttach(builder, attachment);
        }

        message.Body = builder.ToMessageBody();

        try
        {
            using var client = new SmtpClient();
            await client.ConnectAsync(smtp.Host, smtp.Port, GetSecureSocketOption(smtp), cancellationToken);

            if (!string.IsNullOrWhiteSpace(smtp.Username))
            {
                await client.AuthenticateAsync(smtp.Username, smtp.Password, cancellationToken);
            }

            var response = await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            return new EmailSendResult
            {
                Accepted = true,
                MessageId = message.MessageId ?? response
            };
        }
        catch (Exception ex)
        {
            return new EmailSendResult
            {
                Accepted = false,
                Error = ex.Message
            };
        }
    }

    private static void AddRecipients(InternetAddressList addressList, IEnumerable<EmailAddress> addresses)
    {
        foreach (var address in addresses.Where(a => !string.IsNullOrWhiteSpace(a.Address)))
        {
            addressList.Add(new MailboxAddress(address.Name ?? string.Empty, address.Address));
        }
    }

    private static void TryAttach(BodyBuilder builder, EmailAttachment attachment)
    {
        if (!string.IsNullOrWhiteSpace(attachment.FilePath) && File.Exists(attachment.FilePath))
        {
            builder.Attachments.Add(attachment.FilePath);
            return;
        }

        if (!string.IsNullOrWhiteSpace(attachment.ContentBase64))
        {
            try
            {
                var contentBytes = Convert.FromBase64String(attachment.ContentBase64);
                var fileName = string.IsNullOrWhiteSpace(attachment.FileName) ? "attachment.bin" : attachment.FileName;
                var contentType = !string.IsNullOrWhiteSpace(attachment.ContentType)
                    ? ContentType.Parse(attachment.ContentType)
                    : new ContentType("application", "octet-stream");

                builder.Attachments.Add(fileName, contentBytes, contentType);
            }
            catch
            {
                // ignore malformed attachments; valid ones still flow through
            }
        }
    }

    private static SecureSocketOptions GetSecureSocketOption(SmtpSettings smtp)
    {
        if (smtp.UseSsl)
        {
            return SecureSocketOptions.SslOnConnect;
        }

        if (smtp.UseStartTls)
        {
            return SecureSocketOptions.StartTls;
        }

        return SecureSocketOptions.Auto;
    }
}
