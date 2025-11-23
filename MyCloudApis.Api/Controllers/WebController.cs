using Microsoft.AspNetCore.Mvc;
using MyCloudApis.Application.Contracts;
using MyCloudApis.Application.Interfaces;

namespace MyCloudApis.Api.Controllers;

[ApiController]
[Route("web")]
public class WebController : ControllerBase
{
    private readonly IUrlRenderService _urlRenderService;

    public WebController(IUrlRenderService urlRenderService)
    {
        _urlRenderService = urlRenderService;
    }

    [HttpPost("render")]
    public async Task<IActionResult> Render([FromBody] UrlRenderRequest request, CancellationToken cancellationToken)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Url))
        {
            return BadRequest("URL is required.");
        }

        var bytes = await _urlRenderService.RenderAsync(request, cancellationToken);
        var fileName = string.IsNullOrWhiteSpace(request.FileName)
            ? (request.AsImage ? $"capture-{DateTime.UtcNow:yyyyMMddHHmmss}.png" : $"capture-{DateTime.UtcNow:yyyyMMddHHmmss}.pdf")
            : request.FileName;

        var contentType = request.AsImage ? "image/png" : "application/pdf";
        return File(bytes, contentType, fileName);
    }
}
