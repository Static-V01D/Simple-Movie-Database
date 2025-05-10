using System.Collections;
using System.Net;
using System.Text;
using System.Web;

namespace SMDB;

public class HttpUtils
{
    public static async Task Respond(HttpListenerResponse res, HttpListenerRequest req,Hashtable options, int statusCode, string body)
    {
       byte[] content = Encoding.UTF8.GetBytes(body);

        res.StatusCode = statusCode;
        res.ContentEncoding = Encoding.UTF8;
        res.ContentType = "text/html";
        res.ContentLength64 = content.LongLength;
        await res.OutputStream.WriteAsync(content);
        res.Close();
    }
     public static async Task Redirect(HttpListenerResponse res, HttpListenerRequest req,Hashtable options, string location)
    {
        string message = (string?) options["message"] ?? "";
        string query = string.IsNullOrWhiteSpace(message) ? "" : "?message=" + HttpUtility.UrlEncode(message);

        res.Redirect(location + query);
        res.Close();

       await Task.CompletedTask;
    }

    public static async Task ReadRequestFormData(HttpListenerRequest req,HttpListenerResponse res, Hashtable options)
    {
        Console.WriteLine("ReadRequestFormData middleware executed");
        
        string type = req.ContentType ?? "";

        if(type.StartsWith("application/x-www-form-urlencoded"))
        {
            using var sr = new StreamReader(req.InputStream, Encoding.UTF8);
            string body = await sr.ReadToEndAsync();
            var formData = HttpUtility.ParseQueryString(body);

            options["req.form"] = formData;
        }
    }

 
}