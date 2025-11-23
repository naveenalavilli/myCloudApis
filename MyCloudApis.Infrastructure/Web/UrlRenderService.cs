using Microsoft.Playwright;
using MyCloudApis.Application.Contracts;
using MyCloudApis.Application.Interfaces;

namespace MyCloudApis.Infrastructure.Web;

/// <summary>
/// Uses Playwright headless Chromium to render URLs to PDF or PNG.
/// </summary>
public class UrlRenderService : IUrlRenderService
{
    public async Task<byte[]> RenderAsync(UrlRenderRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Url))
        {
            throw new ArgumentException("URL is required.", nameof(request.Url));
        }

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });

        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync(request.Url, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });

        if (request.AsImage)
        {
            return await page.ScreenshotAsync(new PageScreenshotOptions { FullPage = true });
        }

        return await page.PdfAsync(new PagePdfOptions { Format = "A4" });
    }
}
