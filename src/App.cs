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

    private int requestId;

    public App()
    {
        string host = "http://localhost:8080/";
        server = new HttpListener();
        server.Prefixes.Add(host);
        requestId = 0;

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
        router.AddPost("/register");
        router.AddGet("/login");
        router.AddGet("/logout");
        router.AddGet("/actors");
        router.AddGet("/movies");

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
    private async Task HandleContextAsync(HttpListenerContext ctx) // Handle the incoming request
    {
        var req = ctx.Request;
        var res = ctx.Response;        
        var options = new Hashtable();
        string error = "";

        res.StatusCode = HttpRouter.RESPONSE_NOT_SENT_YET;   
        DateTime startTime = DateTime.UtcNow;
        requestId++;

        try
        {                   
            await router.Handle(req, res, options);
        }
        catch (Exception ex)
        {            
            error = ex.ToString();         

            if(res.StatusCode == HttpRouter.RESPONSE_NOT_SENT_YET) // Handle 500 Internal Server Error
            {
                if(Environment.GetEnvironmentVariable("DEVELOPMENT_MODE")!= "Production")
                {
                  string html = HtmlTemplates.Base("SimpleMDB", "Error page", ex.ToString());
                  await HttpUtils.Respond(res, req, options, (int)HttpStatusCode.InternalServerError, html);
                }
                else 
                {
                    string html = HtmlTemplates.Base("SimpleMDB", "Error page", "An error occurred. Please try again later.");
                    await HttpUtils.Respond(res, req, options, (int)HttpStatusCode.InternalServerError, html);
                }
              
            }
        }
        finally
        {           
            if(res.StatusCode == HttpRouter.RESPONSE_NOT_SENT_YET) // If the response has not been sent yet, send a 404 Not Found response
            {
                // Handle 404 Not Found
                string html = HtmlTemplates.Base("SimpleMDB", "Page Not Found", "Resource not found"); 
                await HttpUtils.Respond(res, req, options, (int)HttpStatusCode.NotFound, html);
            }

            string rid = req.Headers["X-Request-ID"] ?? requestId.ToString().PadLeft(6, ' ');
            TimeSpan elapsedTime = DateTime.UtcNow - startTime;           

            Console.WriteLine($"Request {rid}: {req.HttpMethod} {req.RawUrl} from {req.UserHostName} --> {res.StatusCode} ({res.ContentLength64} bytes) in {elapsedTime.TotalMilliseconds} ms Error: \"{error}\"");
           
        }
    }
      
}

