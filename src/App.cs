namespace SMDB;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

public class App
{
    private HttpListener server;

    public App()
    {
        string host = "http://localhost:8080/";
        server = new HttpListener();
        server.Prefixes.Add(host);

        Console.WriteLine($"Server started on " + host);
       
    }

    public async Task Start()
    {
        server.Start();
        while (server.IsListening)
        {
           var ctx = server.GetContext();
           await HandleContextAsync(ctx);
        }
        
    }

    public void Stop()
    {
        server.Stop();
        server.Close();
    }
    private async Task HandleContextAsync(HttpListenerContext ctx)
    {
        var req = ctx.Request;
        var res = ctx.Response;
        
        if(req.HttpMethod == "GET" && req.Url!.AbsolutePath == "/")
        {
            string html = "HELLO";
            byte[] content = Encoding.UTF8.GetBytes(html);

            res.StatusCode = (int)HttpStatusCode.OK;
            res.ContentEncoding = Encoding.UTF8;
            res.ContentType = "text/plain";
            res.ContentLength64 = content.LongLength;
            await res.OutputStream.WriteAsync(content);
            res.Close();
        }
        
    }
}
