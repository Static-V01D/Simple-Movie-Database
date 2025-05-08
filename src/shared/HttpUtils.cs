using System.Collections;
using System.Net;
using System.Text;

namespace SMDB;

public class HttpUtils
{
    public static async Task Respond(HttpListenerResponse res, HttpListenerRequest req,Hashtable options, string body,int statusCode)
    {
       byte[] content = Encoding.UTF8.GetBytes(body);

        res.StatusCode = statusCode;
        res.ContentEncoding = Encoding.UTF8;
        res.ContentType = "text/html";
        res.ContentLength64 = content.LongLength;
        await res.OutputStream.WriteAsync(content);
        res.Close();
    }
}