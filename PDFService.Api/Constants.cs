namespace PDFService.Api
{
    public static class Constants
    {
        public const string ApiKeyHeaderName = "X-API-Key";
        public const string ApiKeyName = "ApiKey";

        public const string HtmlStyle =
            @"<style>
                body { 
                    font-size: 16px;
                }
                // You can put any additional styles here, such as font-family, colors, etc. This is just a basic example to set the font size for the entire document.
            </style>";

        // html template with placefolders for the style and the body content
        public const string HtmlTemplate = 
            @"<!DOCTYPE html>
            <html>
            <head>
                <title></title>
                {0}
            </head>
            <body>
                {1}
            </body>
            </html>";
    }
}
