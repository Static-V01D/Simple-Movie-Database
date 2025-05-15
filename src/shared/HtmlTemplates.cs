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
        <link rel=""stylesheet"" a href=""./styles/main.css"">
        <script type=""text/javascript"" a href=""scripts/main.js"" defer></script>
        </head>
        <body>
            <h1 class = ""header"">{header}</h1>
            <div class = ""content"">{content}</div>
            <div class = ""message"">{message}</div>
        </body>
        </html>";
    }
   

}