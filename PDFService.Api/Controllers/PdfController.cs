using System.Text;
using PDFService.Api.Models;
using PDFService.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ApplicationInsights;
using PDFService.Api.Services;
using PDFService.Services.Models;
using Polly;
using Polly.Retry;
using JsonSerializer = System.Text.Json.JsonSerializer;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;

namespace PDFService.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PdfController : ControllerBase
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly IPdfService _pdfService;
        private readonly IApiKeyValidation _apiKeyValidation;
        private readonly IHttpClientFactory _clientFactory;
        private readonly PdfOptions _pdfOptions;
        private readonly HtmlService _htmlService;

        public PdfController(IPdfService pdfService,
                             TelemetryClient telemetryClient,
                             IApiKeyValidation apiKeyValidation,
                             IHttpClientFactory clientFactory,
                             PdfOptions options,
                             HtmlService htmlService)
        {
            this._telemetryClient = telemetryClient;
            this._pdfService = pdfService;
            this._apiKeyValidation = apiKeyValidation;
            this._clientFactory = clientFactory;
            this._pdfOptions = options;
            this._htmlService = htmlService;
        }


        [HttpPost(Name = "ConvertHtmlToPdf")]
        public async Task<IActionResult> ConvertHtmlToPdf([FromBody] PdfRequestForConversion request)
        {
            var convertible = _htmlService.ConvertBase64ToHtml(request.Base64OfPdf);
            var response = _htmlService.ConvertPdfFromHtml(convertible);

            return Ok(response);
        }


        [HttpPost]
        [Route("/mergepdfs")]
        public async Task<IActionResult> MergePdfs([FromBody] MergePdfRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.HtmlHeader) || string.IsNullOrEmpty(request.PdfBase64))
            {
                return BadRequest("Both HTML and PDF base64 strings are required.");
            }

            var convertible = _htmlService.ConvertBase64ToHtml(request.HtmlHeader);
            var convertedHeaderHtml = _htmlService.ConvertPdfFromHtml(convertible);
            try
            {
                byte[] pdfBytes1 = Convert.FromBase64String(convertedHeaderHtml!);
                byte[] pdfBytes2 = Convert.FromBase64String(request.PdfBase64);

                using (var outputStream = new MemoryStream())
                {
                    using (var pdfDocument1 = new PdfDocument(new PdfReader(new MemoryStream(pdfBytes1)), new PdfWriter(outputStream)))
                    using (var pdfDocument2 = new PdfDocument(new PdfReader(new MemoryStream(pdfBytes2))))
                    {
                        var merger = new PdfMerger(pdfDocument1);
                        merger.Merge(pdfDocument2, 1, pdfDocument2.GetNumberOfPages());
                    }

                    var mergedPdfBase64 = Convert.ToBase64String(outputStream.ToArray());

                    return Ok(new { MergedPdfBase64 = mergedPdfBase64 });
                }
            }
            catch (Exception e)
            {
                _telemetryClient.TrackException(e, new Dictionary<string, string>
                                                    {
                                                        { "Description", "Error merging PDFs" }
                                                    });
                return StatusCode(500, "Error merging PDFs");
            }
        }


        [HttpPost]
        [Route("/mergemultiplepdfs")]
        public async Task<IActionResult> MergeMultiplePdfs([FromBody] MergeMultiplePdfRequest request)
        {
            if (request.PdfsInBase64 == null || request.PdfsInBase64.Length == 0)
            {
                return BadRequest("At least one base64 PDF string is required.");
            }

            try
            {
                using (var outputStream = new MemoryStream())
                {
                    using (var pdfDocument = new PdfDocument(new PdfWriter(outputStream)))
                    {
                        var merger = new PdfMerger(pdfDocument);

                        foreach (var base64Pdf in request.PdfsInBase64)
                        {
                            byte[] pdfBytes = Convert.FromBase64String(base64Pdf);
                            using (var pdfStream = new MemoryStream(pdfBytes))
                            using (var pdfDoc = new PdfDocument(new PdfReader(pdfStream)))
                            {
                                merger.Merge(pdfDoc, 1, pdfDoc.GetNumberOfPages());
                            }
                        }
                    }

                    var mergedPdfBase64 = Convert.ToBase64String(outputStream.ToArray());

                    return Ok(new { MergedPdfBase64 = mergedPdfBase64 });
                }
            }
            catch (Exception e)
            {
                _telemetryClient.TrackException(e);
                return StatusCode(500, "Error merging PDFs");
            }
        }
    }


    public class MergeMultiplePdfRequest
    {
        public required string[] PdfsInBase64 { get; set; }
    }


    public class MergePdfRequest
    {
        public required string HtmlHeader { get; set; }
        public required string PdfBase64 { get; set; }
    }
}
