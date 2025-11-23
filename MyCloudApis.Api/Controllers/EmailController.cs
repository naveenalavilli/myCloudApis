using Microsoft.AspNetCore.Mvc;
using MyCloudApis.Application.Contracts;
using MyCloudApis.Application.Interfaces;

namespace MyCloudApis.Api.Controllers;

[ApiController]
[Route("email")]
public class EmailController : ControllerBase
{
    private readonly IEmailService _emailService;

    public EmailController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send([FromBody] SendEmailRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return BadRequest("Email payload is required.");
        }

        var sendResult = await _emailService.SendAsync(request, cancellationToken);
        if (!sendResult.Accepted)
        {
            return BadRequest(sendResult.Error ?? "Email could not be delivered.");
        }

        return Ok(new
        {
            messageId = sendResult.MessageId,
            status = "queued"
        });
    }
}
