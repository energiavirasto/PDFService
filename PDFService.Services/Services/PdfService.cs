using PDFService.Services.Models;
using iText.Html2pdf;
using iText.Html2pdf.Resolver.Font;
using iText.IO.Font;
using iText.Kernel.Pdf;
using iText.Layout.Font;

namespace PDFService.Services.Services
{
    public class PdfService : IPdfService
    {
        private ConverterProperties converterProperties;
        private readonly PdfOptions _pdfOptions;

        private static string CURRENT_DIR = Directory.GetCurrentDirectory();

        public PdfService(PdfOptions pdfOptions)
        {
            _pdfOptions = pdfOptions;

            // Create converter properties
            converterProperties = new ConverterProperties();
        }
        public Stream ConvertHtmlToPdf(string html)
        {

            // Set the base uri
            converterProperties.SetBaseUri(CURRENT_DIR);

            SetConformanceLevel();

            SetColorProfile();

            SetFonts();

            var pdfStream = ConvertData(html);

            return pdfStream;
        }

        private void SetConformanceLevel()
        {
            // Set the PDF/A conformance level
            converterProperties.SetPdfAConformanceLevel(PdfAConformanceLevel.PDF_A_2B);
        }

        private void SetColorProfile()
        {
            // Set the color profile
            var inputStream = new FileStream($"{CURRENT_DIR}/Resources/{_pdfOptions.ColorProfile}", FileMode.Open, FileAccess.Read);
            converterProperties.SetDocumentOutputIntent(new PdfOutputIntent("Custom", "", "http://www.color.org",
                "sRGB IEC61966-2.1", inputStream));
        }

        private void SetFonts()
        {
            var fonts = _pdfOptions.FontName.Split(',');
            
            // Set the font provider
            FontProvider fontProvider = new DefaultFontProvider(false, false, false);
            foreach (var font in fonts) {

                FontProgram fontProgram = FontProgramFactory.CreateFont($"{CURRENT_DIR}/Fonts/{font}");
                fontProvider.AddFont(fontProgram);
            }
            converterProperties.SetFontProvider(fontProvider);

            // Set the charset
            converterProperties.SetCharset("UTF-8");
        }

        private Stream ConvertData(string html)
        {
            // Convert html to pdf and return stream
            using (var memoryStream = new MemoryStream())
            {
                HtmlConverter.ConvertToPdf(html, memoryStream, converterProperties);

                // Copy the contents to another stream before the original stream is closed
                var outputStream = new MemoryStream(memoryStream.ToArray());
                return outputStream;
            }
        }
    }
}
