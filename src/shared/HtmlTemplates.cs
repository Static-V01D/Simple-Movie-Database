namespace SMDB;

public class HtmlTemplates
{
    public static string Base(string title, string header, string content, string message = "")
    {
        return $@"
        <html>
        <head>
        <title>{title}</title>
        <link rel=""icon"" type=""image/x-icon"" href=""favicon.png"">        
        <style>
        body {{ font-family: Arial, sans-serif; background: #f8f8f8; }}
        .header {{ color: black; text-align: center; }}
        .content {{ margin: 20px auto; max-width: 800px; background: #fff; padding: 20px; border-radius: 8px; }}
        .message {{ color: red; font-weight: bold; margin: 10px; text-align: center; }}
        </style>
        <script type=""text/javascript"" a href=""/scripts/main.js"" defer></script>
        </head>
        <body>
            <h1 class = ""header"">{header}</h1>
            <div class = ""content"">{content}</div>
            <div class = ""message"">{message}</div>
        </body>
        </html>";
    }


}


