using HtmlRendererCore.PdfSharp;
using MyCloudApis.Application.Interfaces;
using PdfSharpCore;
using PdfSharpCore.Fonts;
using PdfSharpCore.Pdf;
using PdfSharpCore.Utils;

namespace MyCloudApis.Infrastructure.Pdf;

/// <summary>
/// Converts simple/medium-complexity HTML into a PDF byte array using PdfSharpCore.
/// </summary>
public class PdfService : IPdfService
{
    public PdfService()
    {
        // Ensure fonts resolve cross-platform without relying on system-installed fonts.
        if (GlobalFontSettings.FontResolver == null)
        {
            GlobalFontSettings.FontResolver = new FontResolver();
        }
    }

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
