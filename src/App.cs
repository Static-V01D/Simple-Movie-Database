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

        //var userRepository = new MockUserRepository();
        var userRepository = new MySQLUserRepository("Server=localhost;Database=simplemdb;Uid=root;Pwd=12345;");
        var userService = new MockUserService(userRepository);
        var authController = new AuthenticationController(userService);
        var userController = new UserController(userService);

        var actorRepository = new MySQLActorRepository("Server=localhost;Database=simplemdb;Uid=root;Pwd=12345;");
        var actorService = new MockActorService(actorRepository);
        var actorController = new ActorController(actorService);

        // var movieRepository = new MockMovieRepository();
        var movieRepository = new MySQLMovieRepository("Server=localhost;Database=simplemdb;Uid=root;Pwd=12345;");
        var movieService = new MockMovieService(movieRepository);
        var movieController = new MovieController(movieService);


        // var actorMovieRepository = new MockActorMovieRepository(actorRepository, movieRepository);
        var actorMovieRepository = new MySQLActorMovieRepository("Server=localhost;Database=simplemdb;Uid=root;Pwd=12345;");
        var actorMovieService = new MockActorMovieService(actorMovieRepository);
        var actorMovieController = new ActorMovieController(actorMovieService, actorService, movieService);

        router = new HttpRouter();
        router.Use(HttpUtils.ServeStaticFile);

        router.AddGet("/", authController.LandingPageGet);
        router.AddGet("/register", authController.RegisterGet);
        router.AddPost("/register", HttpUtils.ReadRequestFormData,authController.RegisterPost); //Fix the issue of Username always being "empty" on Register, had to "Force" ReadRequestData to called before register
        router.AddGet("/login", authController.LoginGet);
        router.AddPost("/login", HttpUtils.ReadRequestFormData, authController.LoginPost);
        router.AddPost("/logout", authController.LogoutPost);//logout


        router.AddGet("/users", authController.CheckAdmin, userController.ViewAllUsers);
        router.AddGet("/users/add", authController.CheckAdmin, userController.AddUserGet);
        router.AddPost("/users/add", authController.CheckAdmin, HttpUtils.ReadRequestFormData, userController.AddUserPost);
        router.AddGet("/users/view", authController.CheckAdmin, userController.ViewUserGet);
        router.AddGet("/users/edit", authController.CheckAdmin, userController.EditUserGet);
        router.AddPost("/users/edit", authController.CheckAdmin, HttpUtils.ReadRequestFormData, userController.EditUserPost);
        router.AddPost("/users/remove", authController.CheckAdmin, userController.RemoveUserPost);


        router.AddGet("/actors", actorController.ViewAllActors);
        router.AddGet("/actors/add", authController.Checkauth, actorController.AddActorGet);
        router.AddPost("/actors/add", authController.Checkauth, HttpUtils.ReadRequestFormData, actorController.AddActorPost);
        router.AddGet("/actors/view", authController.Checkauth, actorController.ViewActorGet);
        router.AddGet("/actors/edit", authController.Checkauth, actorController.EditActorGet);
        router.AddPost("/actors/edit", authController.Checkauth, HttpUtils.ReadRequestFormData, actorController.EditActorPost);
        router.AddPost("/actors/remove", authController.Checkauth, actorController.RemoveActorPost);

        router.AddGet("/movies", movieController.ViewAllMovies);
        router.AddGet("/movies/add", authController.Checkauth, movieController.AddMovieGet);
        router.AddPost("/movies/add", authController.Checkauth, HttpUtils.ReadRequestFormData, movieController.AddMoviePost);
        router.AddGet("/movies/view", authController.Checkauth, movieController.ViewMovieGet);
        router.AddGet("/movies/edit", authController.Checkauth,movieController.EditMovieGet);
        router.AddPost("/movies/edit", authController.Checkauth, HttpUtils.ReadRequestFormData, movieController.EditMoviePost);
        router.AddPost("/movies/remove", authController.Checkauth, movieController.RemoveMoviePost);

        router.AddGet("/actors/movies", authController.Checkauth, actorMovieController.ViewAllMoviesByActor);
        router.AddGet("/actors/movies/add", authController.Checkauth, actorMovieController.AddMoviesByActorGet);
        router.AddPost("/actors/movies/add", authController.Checkauth, HttpUtils.ReadRequestFormData, actorMovieController.AddMoviesByActorPost);
        router.AddPost("/actors/movies/remove", authController.Checkauth, actorMovieController.RemoveMoviesByActorPost);


        router.AddGet("/movies/actors", authController.Checkauth, actorMovieController.ViewAllActorsInMovie);
        router.AddGet("/movies/actors/add", authController.Checkauth, actorMovieController.AddActorsByMovieGet); // <-- add this
        router.AddPost("/movies/actors/add", authController.Checkauth, HttpUtils.ReadRequestFormData, actorMovieController.AddActorsByMoviePost);
        router.AddPost("/movies/actors/remove", authController.Checkauth, actorMovieController.RemoveActorsByMoviePost);
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

