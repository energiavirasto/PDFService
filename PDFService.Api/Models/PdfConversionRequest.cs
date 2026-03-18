namespace PDFService.Api.Models
{
    public class PdfConversionRequest
    {
        public required List<FileToConvert> FilesToConvert { get; set; }
    }

    public class PdfRequestForConversion
    {
        public required string Base64OfPdf { get; set; }

        public required string FileName { get; set; }

    }

    public class FileToConvert
    {
        // File content is the html content to convert to pdf, in base64 format
        public required string FileContent { get; set; }
        public required string FileName { get; set; }
    }
}
