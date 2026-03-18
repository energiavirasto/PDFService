using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFService.Services.Services
{
    public interface IPdfService
    {
        // Convert html to pdf
        Stream ConvertHtmlToPdf(string html);

        // Create base64 string from file
        static string CreateBase64(Stream input)
        {
            // Create base64 string from file
            byte[] bytes = new byte[input.Length];
            input.Read(bytes, 0, (int)input.Length);
            return Convert.ToBase64String(bytes);
        }

        // Decode base64 string to stream
        static Stream DecodeBase64(string input)
        {
            // Decode base64 string to stream
            byte[] bytes = Convert.FromBase64String(input);
            return new MemoryStream(bytes);
        }

        // Get string from stream
        static string GetStringFromStream(Stream input)
        {
            // Get string from stream
            using (StreamReader reader = new StreamReader(input))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
