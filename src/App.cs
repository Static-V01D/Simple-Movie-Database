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

        var ActorRepository = new MockActorRepository();
        var ActorService = new MockActorService(ActorRepository);       
        var ActorController = new ActorController(ActorService);

        var MovieRepository = new MockMovieRepository();
        var MovieService = new MockMovieService(MovieRepository);       
        var MovieController = new MovieController(MovieService);

        router = new HttpRouter();
        router.Use(HttpUtils.ServeStaticFile);

        router.AddGet("/", authController.LandingPageGet);

        router.AddGet("/users", userController.ViewAllUsers);
        router.AddGet("/users/add", userController.AddUserGet);
        router.AddPost("/users/add", HttpUtils.ReadRequestFormData, userController.AddUserPost);
        router.AddGet("/users/view", userController.ViewUserGet);
        router.AddGet("/users/edit", userController.EditUserGet);
        router.AddPost("/users/edit", HttpUtils.ReadRequestFormData, userController.EditUserPost);
        router.AddPost("/users/remove", userController.RemoveUserPost);


        router.AddGet("/Actors", ActorController.ViewAllActors);
        router.AddGet("/Actors/add", ActorController.AddActorGet);
        router.AddPost("/Actors/add", HttpUtils.ReadRequestFormData, ActorController.AddActorPost);
        router.AddGet("/Actors/view", ActorController.ViewActorGet);
        router.AddGet("/Actors/edit", ActorController.EditActorGet);
        router.AddPost("/Actors/edit", HttpUtils.ReadRequestFormData, ActorController.EditActorPost);
        router.AddPost("/Actors/remove", ActorController.RemoveActorPost);

        router.AddGet("/Movies", MovieController.ViewAllMovies);
        router.AddGet("/Movies/add", MovieController.AddMovieGet);
        router.AddPost("/Movies/add", HttpUtils.ReadRequestFormData, MovieController.AddMoviePost);
        router.AddGet("/Movies/view", MovieController.ViewMovieGet);
        router.AddGet("/Movies/edit", MovieController.EditMovieGet);
        router.AddPost("/Movies/edit", HttpUtils.ReadRequestFormData, MovieController.EditMoviePost);
        router.AddPost("/Movies/remove", MovieController.RemoveMoviePost);
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

        var rid = req.Headers["X-request-ID"] ?? requestId.ToString().PadLeft(6,' ');
        var method = req.HttpMethod;
        var rawUrl = req.RawUrl;
        var remoteEndpoint = req.RemoteEndPoint;
        


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

           
            TimeSpan elapsedTime = DateTime.UtcNow - startTime;          

            Console.WriteLine($"Request {rid}: {method} {rawUrl} from {remoteEndpoint} --> {res.StatusCode} ({res.ContentLength64} bytes) [{res.ContentType}] in {elapsedTime.TotalMilliseconds} ms Error: \"{error}\"");
           
        }
    }
      
}

