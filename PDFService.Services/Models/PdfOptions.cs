using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFService.Services.Models
{
    public class PdfOptions
    {
        public required string FontName { get; set; }
        public required string ColorProfile { get; set; }
    }
}
