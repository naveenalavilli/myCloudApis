# Developers Guide

## Solution layout (clean architecture)
- `MyCloudApis.Domain` — core types (`EmailAddress`, `EmailAttachment`, `SmtpSettings`).
- `MyCloudApis.Application` — service contracts (`IPdfService`, `IEmailService`, `IExcelExportService`) and request/response DTOs.
- `MyCloudApis.Infrastructure` — implementations using industry packages (MailKit for SMTP, HtmlRendererCore.PdfSharp/PdfSharpCore for HTML→PDF, ClosedXML for Excel) and DI registration via `DependencyInjection`.
- `MyCloudApis.Api` — minimal APIs + Swagger UI wiring.
- `MyCloudApis.Tests` — xUnit tests for services.
Controllers are under `MyCloudApis.Api/Controllers` for PDF, Email, Excel, CSV, Templates, and Web rendering.

## Setup
```bash
dotnet restore
dotnet build
```
Local run:
```bash
dotnet run --project MyCloudApis.Api
```
Swagger: `https://localhost:7274/swagger`.

Playwright browser (needed for `/web/render` at runtime):
```bash
npx playwright install chromium
```

## Configuration
- Default SMTP settings: `MyCloudApis.Api/appsettings.json` under `Email:DefaultSmtp`. Update host/port/credentials/TLS/from address.
- Per-request SMTP overrides are supported on `/email/send` by including a `smtp` object in the payload.
- `nuget.config` pins package source to `nuget.org` (avoids unreachable private feeds).

## Testing
```bash
dotnet test MyCloudApis.sln
```
Current tests cover:
- PDF generation returns bytes.
- Excel export writes rows.
- Email service validation (missing recipients/SMTP).
- CSV export/import round-trip.

## Implementation notes
- PDF: `MyCloudApis.Infrastructure/Pdf/PdfService` uses HtmlRendererCore.PdfSharp; good for typical HTML. For complex web layouts, consider swapping to a headless browser renderer (Playwright/Puppeteer) behind the same interface.
- Email: `MyCloudApis.Infrastructure/Email/EmailService` uses MailKit; supports attachments via path or base64.
- Excel: `MyCloudApis.Infrastructure/Excel/ExcelExportService` uses ClosedXML; expects flat key/value rows and auto-adjusts columns.
- Templates: `MyCloudApis.Infrastructure/Templates/TemplateService` renders Razor templates from strings; feed JSON models from clients.
- Web capture: `MyCloudApis.Infrastructure/Web/UrlRenderService` uses Playwright headless Chromium to render URLs to PDF/PNG.
- CSV: `MyCloudApis.Infrastructure/Csv/CsvService` supports simple export/import with custom delimiter (import expects `multipart/form-data`).

## CI
- GitHub Actions workflow at `.github/workflows/dotnet.yml` runs restore, build (Release), and tests on push/PR to `main`/`master`.
- Extend with matrix (OS/config), code coverage upload, or Playwright browser install if headless capture is exercised in tests.
- Linux runners install `libgdiplus` and `fonts-dejavu-core` for HtmlRenderer/PDF tests.

## Extension points
- Add new features by defining contracts in Application and implementing in Infrastructure; register in `DependencyInjection`.
- Add auth/rate limiting in the API layer before public exposure.
- If adding workflows, consider MediatR or a similar orchestrator in Application.
