using Microsoft.Extensions.Options;
using MyCloudApis.Application.Contracts;
using MyCloudApis.Domain;
using MyCloudApis.Infrastructure.Email;

namespace MyCloudApis.Tests;

public class EmailServiceTests
{
    [Fact]
    public async Task SendAsync_NoRecipients_ReturnsError()
    {
        var options = Options.Create(new EmailOptions());
        var service = new EmailService(options);

        var result = await service.SendAsync(new SendEmailRequest());

        Assert.False(result.Accepted);
        Assert.False(string.IsNullOrWhiteSpace(result.Error));
    }

    [Fact]
    public async Task SendAsync_MissingSmtp_ReturnsError()
    {
        var options = Options.Create(new EmailOptions());
        var service = new EmailService(options);

        var request = new SendEmailRequest
        {
            To = [new EmailAddress { Address = "test@example.com" }]
        };

        var result = await service.SendAsync(request);

        Assert.False(result.Accepted);
        Assert.Contains("SMTP", result.Error, StringComparison.OrdinalIgnoreCase);
    }
}
