using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Web;

namespace SMDB;

public class HttpUtils
{
    public static void AddOptions(Hashtable options, string name, string key, string value)
    {
       var prop = (NameValueCollection?) options[name] ?? [];

       options[name] = prop;

       prop[key] = value;

    }
     public static void AddOptions(Hashtable options, string name, NameValueCollection entries)
    {
       var prop = (NameValueCollection?) options[name] ?? [];

       options[name] = prop;

       prop.Add(entries);

    }
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
       var redirectProps = (NameValueCollection?) options["redirect"] ?? [];
       var query = new List<string>();
       var append = location.Contains('?') ? '&' : '?';
        
        foreach(var key in redirectProps.AllKeys)
        {
            query.Add($"{HttpUtility.UrlEncode(key)}={HttpUtility.UrlEncode(redirectProps[key])}");
        }        

        res.Redirect(location + append + string.Join('&', query));
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

    public static readonly NameValueCollection SUPPORTED_IANA_MIME_TYPES = new()
    {
        {".css", "text/css"},
        {".js", "text/javascript"}
    };

   public static async Task ServeStaticFile(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
  {
    string filename = req.Url!.AbsolutePath ?? "";
    string filepath = Path.Combine(Environment.CurrentDirectory, "static", filename.Trim('/', '\\'));
    string fullpath = Path.GetFullPath(filepath);

    if (File.Exists(fullpath))
    {
        string ext = Path.GetExtension(fullpath);
        string type = SUPPORTED_IANA_MIME_TYPES[ext] ?? "application/octet-stream";

        using var fs = File.OpenRead(fullpath);

        
        res.StatusCode = (int)HttpStatusCode.OK;
        res.ContentType = type;
        res.ContentLength64 = fs.Length;

        await fs.CopyToAsync(res.OutputStream);
        res.Close();
    }   
  }
}

 

