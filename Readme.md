# PDFService

A containerized .NET 8 REST API for HTML-to-PDF conversion and PDF merging, built on [iText 7](https://github.com/itext/itext-dotnet).

A fully stand alone containerized PDF/A creation and merging tool.

## License

This project uses the [iText 7 library](https://itextpdf.com/), which is dual licensed as **AGPL/Commercial software**.

- **AGPL**: The GNU Affero General Public License v3.0 is a copyleft license, meaning any derivative work must also be licensed under the same terms. AGPL is a free/open-source software license, however, this does not mean the software is gratis.
- **Commercial**: If you are using iText in software or a service which cannot comply with the AGPL terms, a [commercial license](https://itextpdf.com/sales) is available that exempts you from such obligations.

See [LICENSE.md](LICENSE.md) for full license details.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Docker (optional, for containerized deployment)

## Getting Started

```bash
cd PDFService.Api
dotnet run
```

The API starts on `http://localhost:5076` by default. Swagger UI is available at `/swagger` in development mode.

## API Endpoints

### POST /Pdf

Converts base64-encoded HTML to a PDF document.

**Request:**
```json
{
  "base64OfPdf": "<base64-encoded HTML>",
  "fileName": "document.pdf"
}
```

**Response:** Base64-encoded PDF string.

### POST /mergepdfs

Merges an HTML header (converted to PDF) with an existing PDF document.

**Request:**
```json
{
  "htmlHeader": "<base64-encoded HTML>",
  "pdfBase64": "<base64-encoded PDF>"
}
```

**Response:**
```json
{
  "mergedPdfBase64": "<base64-encoded merged PDF>"
}
```

### POST /mergemultiplepdfs

Merges multiple PDF documents into one.

**Request:**
```json
{
  "pdfsInBase64": [
    "<base64-encoded PDF 1>",
    "<base64-encoded PDF 2>"
  ]
}
```

**Response:**
```json
{
  "mergedPdfBase64": "<base64-encoded merged PDF>"
}
```

## Configuration

Configuration is managed via `appsettings.json`:

- **PdfOptions:FontName** - Comma-separated list of font files (located in `Fonts/`)
- **PdfOptions:ColorProfile** - ICC color profile for PDF/A-2B conformance (located in `Resources/`)
- **ApiKey** - API key for authentication (via `X-API-Key` header)

## Running Tests

```bash
cd ..
dotnet test
```

## Docker

```bash
docker build -t integrations-pdf-service .
docker run -p 8080:8080 integrations-pdf-service
```
