namespace SMDB;
using System.Collections;
using System.Net;
using System.Text;

public class HttpRouter
{
    public static readonly int RESPONSE_NOT_SENT_YET = 757;
    private List<HttpMiddleware> middlewares;
    private List<(string, string, HttpMiddleware [] middlewares)> endpoints;

    public HttpRouter()
    {
        middlewares = [];
        endpoints = [];
    }

    public void Use(params HttpMiddleware [] middleware)
    {
        this.middlewares.AddRange(middlewares);
    }

    public void AddEndpoint(string method, string path, params HttpMiddleware [] middlewares)
    {
        this.endpoints.Add((method, path, middlewares));
    }

    public void AddGet(string route, params HttpMiddleware [] middlewares)
    {
        this.AddEndpoint("GET", route, middlewares);
    }
    public void AddPost(string route, params HttpMiddleware [] middlewares)
    {
        this.AddEndpoint("POST", route, middlewares);
    }
    public void AddPut(string route, params HttpMiddleware [] middlewares)
    {
        this.AddEndpoint("PUT", route, middlewares);
    }
    public void AddDelete(string route, params HttpMiddleware [] middlewares)
    {
        this.AddEndpoint("DELETE", route, middlewares);
    }

    public async Task Handle(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        

        foreach(var middleware in middlewares)
        {
            await middleware(req, res, options);

            if (res.StatusCode != RESPONSE_NOT_SENT_YET)
            {
                return;
            }
        }

        foreach(var(method, route, middlewares) in endpoints)
        {
            if(req.HttpMethod == method && req.Url!.AbsolutePath == route)
            {
                foreach(var middleware in middlewares)
                {
                    await middleware(req, res, options);

                    if (res.StatusCode != RESPONSE_NOT_SENT_YET)
                    {
                        return;
                    }
                }
            }
        }

       
    }
}