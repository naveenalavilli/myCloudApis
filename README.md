# MyCloudApis

Public-facing API for three utilities:
- **HTML → PDF**: Post HTML or a local file path and receive a PDF.
- **Email send**: SMTP-backed email delivery with optional attachments.
- **JSON → Excel**: Convert JSON rows into an `.xlsx` file.
- **Templating**: Render Razor templates with JSON models to HTML or PDF.
- **URL capture**: Render a URL to PDF or PNG using headless Chromium.
- **CSV import/export**: Two-way CSV helpers alongside Excel.

Swagger UI: `https://localhost:7274/swagger` (when running locally).

## Quick use
1) Restore/build/run:
```bash
dotnet restore
dotnet run --project MyCloudApis.Api
```
2) Open Swagger at the URL above and exercise endpoints, or use the examples in `MyCloudApis.Api/MyCloudApis.Api.http`.

## CI
- GitHub Actions workflow `.github/workflows/dotnet.yml` restores, builds, and tests the solution on pushes/PRs to `main`/`master`.

## Endpoints
- `POST /pdf/from-html`  
  Body: `{ "htmlContent": "<h1>Hello</h1>", "fileName": "doc.pdf" }` (or `htmlFilePath` instead of `htmlContent`). Returns PDF bytes.
- `POST /email/send`  
  Body includes recipients, subject/body, optional attachments, and optional SMTP override. Uses default SMTP from configuration when not provided.
- `POST /excel/export`  
  Body: `{ "fileName": "report.xlsx", "rows": [ { "Id":1, "Name":"Ada" } ] }`. Returns an `.xlsx` file.
- `POST /template/render-html` — Render Razor template + model to HTML string.
- `POST /template/pdf` — Render template + model to PDF.
- `POST /web/render` — Render a URL to PDF (default) or PNG (`asImage: true`).
- `POST /csv/export` — Export rows to CSV with optional delimiter.
- `POST /csv/import` — Upload CSV as `multipart/form-data` (`file` field) → JSON rows.

## Configuration
SMTP defaults live in `MyCloudApis.Api/appsettings.json` under `Email:DefaultSmtp`. Replace the placeholder Gmail settings with your own (host, port, username/password, TLS mode, from address). You can also supply `smtp` settings per request on `/email/send`.

## Notes
- Email requires valid SMTP credentials (e.g., Gmail app password).
- HTML-to-PDF supports common HTML/CSS; extremely complex layouts may need a headless-browser engine.
- JSON→Excel expects an array of objects with flat key/value pairs.
