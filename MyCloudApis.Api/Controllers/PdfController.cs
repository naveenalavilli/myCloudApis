using Microsoft.AspNetCore.Mvc;
using MyCloudApis.Application.Contracts;
using MyCloudApis.Application.Interfaces;

namespace MyCloudApis.Api.Controllers;

[ApiController]
[Route("pdf")]
public class PdfController : ControllerBase
{
    private readonly IPdfService _pdfService;

    public PdfController(IPdfService pdfService)
    {
        _pdfService = pdfService;
    }

    [HttpPost("from-html")]
    public async Task<IActionResult> FromHtml([FromBody] PdfFromHtmlRequest request, CancellationToken cancellationToken)
    {
        if (request == null || (string.IsNullOrWhiteSpace(request.HtmlContent) && string.IsNullOrWhiteSpace(request.HtmlFilePath)))
        {
            return BadRequest("Provide inline HTML in 'htmlContent' or a local path in 'htmlFilePath'.");
        }

        var html = request.HtmlContent;

        if (string.IsNullOrWhiteSpace(html) && !string.IsNullOrWhiteSpace(request.HtmlFilePath))
        {
            if (!System.IO.File.Exists(request.HtmlFilePath))
            {
                return BadRequest($"HTML file not found at path: {request.HtmlFilePath}");
            }

            html = await System.IO.File.ReadAllTextAsync(request.HtmlFilePath, cancellationToken);
        }

        if (string.IsNullOrWhiteSpace(html))
        {
            return BadRequest("HTML content is empty.");
        }

        var pdfBytes = await _pdfService.FromHtmlAsync(html, cancellationToken);
        var fileName = string.IsNullOrWhiteSpace(request.FileName)
            ? $"document-{DateTime.UtcNow:yyyyMMddHHmmss}.pdf"
            : request.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)
                ? request.FileName
                : $"{request.FileName}.pdf";

        return File(pdfBytes, "application/pdf", fileName);
    }
}
