using Microsoft.AspNetCore.Mvc;
using MyCloudApis.Application.Contracts;
using MyCloudApis.Application.Interfaces;

namespace MyCloudApis.Api.Controllers;

[ApiController]
[Route("template")]
public class TemplateController : ControllerBase
{
    private readonly ITemplateService _templateService;
    private readonly IPdfService _pdfService;

    public TemplateController(ITemplateService templateService, IPdfService pdfService)
    {
        _templateService = templateService;
        _pdfService = pdfService;
    }

    [HttpPost("render-html")]
    public async Task<IActionResult> RenderHtml([FromBody] TemplateRenderRequest request, CancellationToken cancellationToken)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Template))
        {
            return BadRequest("Template is required.");
        }

        var html = await _templateService.RenderHtmlAsync(request, cancellationToken);
        return Ok(new { html });
    }

    [HttpPost("pdf")]
    public async Task<IActionResult> RenderPdf([FromBody] TemplateRenderRequest request, CancellationToken cancellationToken)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Template))
        {
            return BadRequest("Template is required.");
        }

        var html = await _templateService.RenderHtmlAsync(request, cancellationToken);
        var pdfBytes = await _pdfService.FromHtmlAsync(html, cancellationToken);
        var fileName = string.IsNullOrWhiteSpace(request.FileName)
            ? $"template-{DateTime.UtcNow:yyyyMMddHHmmss}.pdf"
            : request.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)
                ? request.FileName
                : $"{request.FileName}.pdf";

        return File(pdfBytes, "application/pdf", fileName);
    }
}
