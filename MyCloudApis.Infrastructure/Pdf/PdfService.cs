using HtmlRendererCore.PdfSharp;
using MyCloudApis.Application.Interfaces;
using PdfSharpCore;
using PdfSharpCore.Pdf;

namespace MyCloudApis.Infrastructure.Pdf;

/// <summary>
/// Converts simple/medium-complexity HTML into a PDF byte array using PdfSharpCore.
/// </summary>
public class PdfService : IPdfService
{
    public Task<byte[]> FromHtmlAsync(string html, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            throw new ArgumentException("HTML content is required to generate a PDF.", nameof(html));
        }

        var config = new PdfGenerateConfig
        {
            PageSize = PageSize.A4,
            MarginLeft = 20,
            MarginRight = 20,
            MarginTop = 20,
            MarginBottom = 20
        };

        using var document = PdfGenerator.GeneratePdf(html, config);
        using var stream = new MemoryStream();
        document.Save(stream, false);
        return Task.FromResult(stream.ToArray());
    }
}
