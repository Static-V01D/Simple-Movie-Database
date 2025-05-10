namespace SMDB;
using System;
using System.Collections;
using System.Net;
using System.Text;
using System.Threading.Tasks;

public class App
{
    private HttpListener server;
    private HttpRouter router;

    public App()
    {
        string host = "http://localhost:8080/";
        server = new HttpListener();
        server.Prefixes.Add(host);

        Console.WriteLine($"Server started on " + host);

        var userRepository = new MockUserRepository();
        var userService = new MockUserService(userRepository);

        var authController = new AuthenticationController(userService);
        var userController = new UserController(userService);

        router = new HttpRouter();
       // router.Use(HttpUtils.ReadRequestFormData);  

        router.AddGet("/", authController.LandingPageGet);
        router.AddGet("/users", userController.ViewAllGet);
        router.AddGet("/users/add", userController.AddGet);
        router.AddPost("/users/add", HttpUtils.ReadRequestFormData, userController.AddPost);
        router.AddGet("/users/view", userController.ViewGet);
        router.AddGet("/users/edit", userController.EditGet);
        router.AddPost("/users/edit", HttpUtils.ReadRequestFormData, userController.EditPost);
        router.AddGet("/users/remove", userController.RemoveGet);
    }

    public async Task Start()
    {
        server.Start();
        while (server.IsListening)
        {
           var ctx = server.GetContext();

           _ = HandleContextAsync(ctx); 
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
        var options = new Hashtable();
       

        try
        {
            res.StatusCode = HttpRouter.RESPONSE_NOT_SENT_YET;           
            await router.Handle(req, res, options);
        }
        catch (Exception ex)
        {            
            Console.Error.WriteLine(ex);           

            if(res.StatusCode == HttpRouter.RESPONSE_NOT_SENT_YET)
            {
                if(Environment.GetEnvironmentVariable("DEVELOPMENT_MODE")!= "Production")
                {
                  await HttpUtils.Respond(res, req, options, (int)HttpStatusCode.InternalServerError, ex.ToString());
                }
                else 
                {
                    await HttpUtils.Respond(res, req, options, (int)HttpStatusCode.InternalServerError, "An error occurred. Please try again later.");
                }
              
            }
        }
        finally
        {
            if(res.StatusCode == HttpRouter.RESPONSE_NOT_SENT_YET)
            {
                res.StatusCode = (int)HttpStatusCode.NotFound;
                res.ContentType = "text/plain";
                byte[] content = Encoding.UTF8.GetBytes("404 Not Found");
                res.ContentLength64 = content.LongLength;
                await res.OutputStream.WriteAsync(content);
                res.Close();
            }
        }
    }
      
}
