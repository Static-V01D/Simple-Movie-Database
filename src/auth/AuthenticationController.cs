namespace SMDB;
using System.Collections;
using System.Net;
using System.Text;

public class AuthenticationController 
{
    public AuthenticationController() 
    {
       
    }

    public async Task LandingPageGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
         string html = "HELLO, WORLD!";
            byte[] content = Encoding.UTF8.GetBytes(html);

            res.StatusCode = (int)HttpStatusCode.OK;
            res.ContentEncoding = Encoding.UTF8;
            res.ContentType = "text/plain";
            res.ContentLength64 = content.LongLength;
            await res.OutputStream.WriteAsync(content);
            res.Close();
    }
    
        
    
}