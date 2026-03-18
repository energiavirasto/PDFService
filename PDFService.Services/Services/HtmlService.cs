using Microsoft.ApplicationInsights;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PDFService.Services.Services
{
    public class HtmlService
    {
        private readonly TelemetryClient _telemetryClient; 
        private readonly IPdfService _pdfService;

        public HtmlService(TelemetryClient telemetryClient, IPdfService pdfService)
        {
            this._telemetryClient = telemetryClient;
            this._pdfService = pdfService;
        }

        /// <summary>
        /// Processes the input HTML string to extract the body content without any <style> sections.
        /// </summary>
        /// <param name="html">The input HTML string.</param>
        /// <returns>The body content without any <style> sections.</returns>
        public string ExtractBodyContentWithoutStyle(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                throw new ArgumentException("Input HTML cannot be null or empty.");
            }

            // Step 1: Remove any <style> sections
            string htmlWithoutStyles = RemoveStyleSections(html);

            // Step 2: Determine if there is a <body> tag
            string bodyContent = ExtractBodyContent(htmlWithoutStyles);

            return bodyContent;
        }

        /// <summary>
        /// Removes all <style> sections from the HTML.
        /// </summary>
        /// <param name="html">The input HTML string.</param>
        /// <returns>HTML without <style> sections.</returns>
        private string RemoveStyleSections(string html)
        {
            string stylePattern = @"<style[^>]*>.*?<\/style>";
            return Regex.Replace(html, stylePattern, string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Extracts the content inside the <body> tag or returns the full HTML if no <body> tag is present.
        /// </summary>
        /// <param name="html">The input HTML string.</param>
        /// <returns>The content inside the <body> tag or the full input if no <body> tag exists.</returns>
        private string ExtractBodyContent(string html)
        {
            string bodyPattern = @"<body[^>]*>(.*?)<\/body>";
            Match match = Regex.Match(html, bodyPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            if (match.Success)
            {
                // Return the content inside the <body> tag
                return match.Groups[1].Value;
            }

            // If no <body> tag exists, assume the entire input is the body content
            return html.Trim();
        }


        public string ConvertBase64ToHtml(string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
            {
                throw new ArgumentException("Input Base64 string cannot be null or empty.", nameof(base64String));
            }

            try
            {
                // Convert the Base64 string to a byte array
                byte[] htmlBytes = Convert.FromBase64String(base64String);

                // Convert the byte array back to a string
                string htmlString = Encoding.UTF8.GetString(htmlBytes);

                return htmlString;
            }
            catch (FormatException ex)
            {
                throw new ArgumentException("The input string is not a valid Base64 encoded string.", ex);
            }
        }

        public void ConvertBase64ToPdf(string base64String, string outputFilePath)
        {
            try
            {
                // Convert base64 string to byte array
                byte[] pdfBytes = Convert.FromBase64String(base64String);

                // Write the byte array to the file
                System.IO.File.WriteAllBytes(outputFilePath, pdfBytes);
            }
            catch (Exception e)
            {
                _telemetryClient.TrackException(e, new Dictionary<string, string>
                {
                    { "Description", "Error converting base64 to PDF" },
                    { "OutputFilePath", outputFilePath }
                });
                throw;
            }
        }

        public string? ConvertPdfFromHtml(string fullHtml)
        {
            // Convert html to pdf, for each file in the request
            try
            {

                var pdfStream = _pdfService.ConvertHtmlToPdf(fullHtml);
                var pdfBase64 = IPdfService.CreateBase64(pdfStream);

                // Set the result for the file
                return pdfBase64;
            }
            catch (Exception e)
            {
                _telemetryClient.TrackException(e);
                return null;
            }
        }
    }
}
