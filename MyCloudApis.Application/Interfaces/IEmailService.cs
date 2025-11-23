using MyCloudApis.Application.Contracts;

namespace MyCloudApis.Application.Interfaces;

public interface IEmailService
{
    Task<EmailSendResult> SendAsync(SendEmailRequest request, CancellationToken cancellationToken = default);
}
