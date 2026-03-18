using System.IO;
using PDFService.Services.Models;
using PDFService.Services.Services;
using iText.Html2pdf;
using iText.Kernel.Pdf;
using iText.Layout.Font;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace PDFService.Tests
{
    public class PdfServiceTests
    {
        private readonly PdfOptions _pdfOptions;

        public PdfServiceTests()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            _pdfOptions = configuration.GetSection("PdfOptions").Get<PdfOptions>()!;
        }

        [Fact]
        public void ConvertHtmlToPdf_ShouldReturnStream()
        {
            // Arrange
            var pdfService = new PdfService(_pdfOptions);
            string html = "<html><body><h1>Hello, World!</h1></body></html>";

            // Act
            var result = pdfService.ConvertHtmlToPdf(html);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<MemoryStream>(result);
        }

        [Fact]
        public void SetConformanceLevel_ShouldSetCorrectLevel()
        {
            // Arrange
            var pdfService = new PdfService(_pdfOptions);

            // Act
            pdfService.ConvertHtmlToPdf("<html></html>");

            // Assert
            var converterPropertiesField = typeof(PdfService).GetField("converterProperties", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var converterProperties = (ConverterProperties)converterPropertiesField!.GetValue(pdfService)!;

            Assert.NotNull(converterProperties);

            var conformanceLevel = converterProperties.GetConformanceLevel();
            
            Assert.Equal(PdfAConformanceLevel.PDF_A_2B, conformanceLevel);
        } 

        [Fact]
        public void SetColorProfile_ShouldSetCorrectProfile()
        {
            // Arrange
            var pdfService = new PdfService(_pdfOptions);

            // Act
            pdfService.ConvertHtmlToPdf("<html></html>");

            // Assert
            var converterPropertiesField = typeof(PdfService).GetField("converterProperties", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var converterProperties = (ConverterProperties)converterPropertiesField!.GetValue(pdfService)!;

            var outputIntentField = typeof(ConverterProperties).GetField("outputIntent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var outputIntent = (PdfOutputIntent)outputIntentField!.GetValue(converterProperties)!;

            Assert.NotNull(outputIntent);
            Assert.Equal("sRGB IEC61966-2.1", outputIntent.GetInfo().GetValue());
        }

        [Fact]
        public void SetFonts_ShouldSetCorrectFonts()
        {
            // Arrange
            var pdfService = new PdfService(_pdfOptions);
            var html = "<html></html>";

            // Act
            pdfService.ConvertHtmlToPdf(html);

            // Assert
            var converterPropertiesField = typeof(PdfService).GetField("converterProperties", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var converterProperties = (ConverterProperties)converterPropertiesField!.GetValue(pdfService)!;

            var fontProvider = converterProperties.GetFontProvider();
            var fonts = fontProvider.GetFontSet().GetFonts();
            
            var fontNames = _pdfOptions.FontName.Split(',');

            // Check that we loaded same number of fonts as in the appsettings.json
            Assert.Equal(fontNames.Length, fonts.Count);
        }     
        
        [Fact]
        public void SetFonts_ShouldContainFonts()
        {
            // Arrange
            var pdfService = new PdfService(_pdfOptions);
            var html = "<html></html>";

            // Act
            pdfService.ConvertHtmlToPdf(html);

            // Assert
            var converterPropertiesField = typeof(PdfService).GetField("converterProperties", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var converterProperties = (ConverterProperties)converterPropertiesField!.GetValue(pdfService)!;

            var fontProvider = converterProperties.GetFontProvider();
            var fonts = fontProvider.GetFontSet().GetFonts();
            
            // There should be fonts loaded
            Assert.NotNull(fonts);  
        }
    }
}
