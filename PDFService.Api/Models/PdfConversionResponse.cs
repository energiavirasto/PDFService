namespace PDFService.Api.Models
{
    public class PdfConversionResponse
    {
        public required List<ConversionResult> ConversionResults { get; set; }
    }

    public class ConversionResult
    {
        // PDF content in base64 format
        public required string FileContent { get; set; }
        public required string FileName { get; set; }
        public bool OperationStatus { get; set; }
        public ErrorDetails? ErrorDetails { get; set; } = null;
    }

    public class ErrorDetails
    {
        public string errorDescription { get; set; } = string.Empty;
        public bool errorPersistence { get; set; } = false;
    }
}
