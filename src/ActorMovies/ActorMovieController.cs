using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Runtime.InteropServices.Marshalling;

namespace SMDB;

public class ActorMovieController
{
    private IActorMovieService actorMovieService;
    private IActorService actorService;

    private IMovieService movieService;

    public ActorMovieController(IActorMovieService actorMovieService, IActorService actorService, IMovieService movieService)
    {
        this.actorMovieService = actorMovieService;
        this.actorService = actorService;
        this.movieService = movieService;
    }

    public async Task ViewAllMoviesByActor(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string message = req.QueryString["message"] ?? "";
        int aid = int.TryParse(req.QueryString["aid"], out int a) && a > 0 ? a : -1;
        if (aid == -1)
        {
            HttpUtils.AddOptions(options, "redirect", "message", "Invalid actor ID.");
            await HttpUtils.Redirect(res, req, options, "/");
            return;
        }
        var result1 = await actorService.Read(aid);
        if (!result1.IsValid || result1.Value == null)
        {
            HttpUtils.AddOptions(options, "redirect", "message", "Actor not found.");
            await HttpUtils.Redirect(res, req, options, "/");
            return;
        }
        int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
        int size = int.TryParse(req.QueryString["size"], out int s) ? s : 1;

        var result2 = await actorMovieService.ReadAllMoviesByActor(aid, page, size);

        if (result2.IsValid)
        {
            var actor = result1.Value!;
            var pagedResult = result2.Value!;
            var amms = pagedResult.Values;
            int totalCount = pagedResult.TotalCount;

            ActorMovie? actorMovie = null;
            string html = ActorMovieHtmlTemplates.ViewAllMoviesByActor(actor, amms, actorMovie, totalCount, page, size);
            string content = HtmlTemplates.Base("SMDB", " View All Movies By Actor Page", html, message);
            await HttpUtils.Respond(res, req, options, (int)HttpStatusCode.OK, html);
        }
        else
        {
            string error = result2.Error!.Message;
            HttpUtils.AddOptions(options, "redirect", "message", error);
            await HttpUtils.Redirect(res, req, options, "/");
        }
    }

    public async Task ViewAllActorsInMovie(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string message = req.QueryString["message"] ?? "";
        int mid = int.TryParse(req.QueryString["mid"], out int m) ? m : 1;
        int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
        int size = int.TryParse(req.QueryString["size"], out int s) ? s : 1;

        var result1 = await movieService.Read(mid);
        var result2 = await actorMovieService.ReadAllActorsByMovie(mid, page, size);

        if (result1.IsValid && result2.IsValid)
        {
            var movie = result1.Value!;
            var pagedResult = result2.Value!;
            var amas = pagedResult.Values;
            int totalCount = pagedResult.TotalCount;

            string html = ActorMovieHtmlTemplates.ViewAllActorsByMovie(movie, amas, totalCount, page, size);
            string content = HtmlTemplates.Base("SMDB", "View All Actors In Movie Page", html, message);
            await HttpUtils.Respond(res, req, options, (int)HttpStatusCode.OK, html);
        }
        else
        {
            string error = result1.IsValid ? "" : result1.Error!.ToString();
            error += result1.IsValid ? "" : result2.Error!.Message;
            HttpUtils.AddOptions(options, "redirect", "message", error);
            await HttpUtils.Redirect(res, req, options, "/");
        }
    }
    //actors/movies/add?aid=1
    public async Task AddMoviesByActorGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string message = req.QueryString["message"] ?? "";

        int aid = int.TryParse(req.QueryString["aid"], out int a) ? a : 1;

        var result1 = await actorService.Read(aid);
        var result2 = await actorMovieService.ReadAllMovies();

        if (result1.IsValid && result2.IsValid)
        {
            var actor = result1.Value!;
            var movies = result2.Value!;

            string html = ActorMovieHtmlTemplates.AddMoviesByActor(actor, movies);
            string content = HtmlTemplates.Base("simpleMDB", "Add Movies By Actor Page", html, message);

            await HttpUtils.Respond(res, req, options, (int)HttpStatusCode.OK, content);
        }
        else
        {
            string error = result1.IsValid ? "" : result1.Error!.Message;
            error += result2.IsValid ? "" : result2.Error!.Message;


            HttpUtils.AddOptions(options, "redirect", "message", error);

            await HttpUtils.Redirect(res, req, options, "/");
        }
    }
    //GET /movies/actors
    public async Task AddActorsByMovieGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string message = req.QueryString["message"] ?? "";
        int mid = int.TryParse(req.QueryString["mid"], out int m) ? m : 1;

        var result1 = await movieService.Read(mid);
        var result2 = await actorMovieService.ReadAllActors();

        if (result1.IsValid && result2.IsValid && result1.Value != null && result2.Value != null)
        {
            var movie = result1.Value;
            var actors = result2.Value;

            string html = ActorMovieHtmlTemplates.AddActorsByMovie(movie, actors);
            string content = HtmlTemplates.Base("SMDB", " Add Actors by movie Page", html, message);
            await HttpUtils.Respond(res, req, options, (int)HttpStatusCode.OK, html);
        }
        else
        {
            string error = "";
            if (!result1.IsValid || result1.Value == null)
                error += "Movie not found. ";
            if (!result2.IsValid || result2.Value == null)
                error += "Actors not found.";
            HttpUtils.AddOptions(options, "redirect", "message", error.Trim());
            await HttpUtils.Redirect(res, req, options, "/");
        }
    }
    public async Task AddMoviesByActorPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var formData = (NameValueCollection?)options["req.form"] ?? [];
        var aid = int.TryParse(formData["aid"], out int a) ? a : -1;
        var mid = int.TryParse(formData["mid"], out int m) ? m : -1;
        var roleName = formData["rolename"] ?? "";

        var result = await actorMovieService.create(aid, mid, roleName);

        if (result.IsValid)
        {
            HttpUtils.AddOptions(options, "redirect", "message", "ActorMovie added successfully!");

            await HttpUtils.Redirect(res, req, options, $"/actors/movies/add?aid={aid}");
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
            HttpUtils.AddOptions(options, "redirect", formData);

            await HttpUtils.Redirect(res, req, options, $"/actors/movies/add?aid={aid}");
        }

    }
    public async Task AddActorsByMoviePost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var formData = (NameValueCollection?)options["req.form"] ?? [];
        var aid = int.TryParse(formData["aid"], out int a) ? a : -1;
        var mid = int.TryParse(formData["mid"], out int m) ? m : -1;
        var roleName = formData["rolename"] ?? "popo";

        // Validate actor and movie exist
        var actorResult = await actorService.Read(aid);
        var movieResult = await movieService.Read(mid);

        if (!actorResult.IsValid || actorResult.Value == null)
        {
            HttpUtils.AddOptions(options, "redirect", "message", "Actor not found.");
            await HttpUtils.Redirect(res, req, options, $"/movies/actors/add");
            return;
        }
        if (!movieResult.IsValid || movieResult.Value == null)
        {
            HttpUtils.AddOptions(options, "redirect", "message", "Movie not found.");
            await HttpUtils.Redirect(res, req, options, $"/movies/actors/add?mid={mid}");
            return;
        }

        var result = await actorMovieService.create(aid, mid, roleName);

        if (result.IsValid)
        {
            HttpUtils.AddOptions(options, "redirect", "message", "ActorMovie added successfully!");
            await HttpUtils.Redirect(res, req, options, $"/movies/actors/add?aid={aid}");
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
            HttpUtils.AddOptions(options, "redirect", formData);
            await HttpUtils.Redirect(res, req, options, $"/movies/actors/add?mid={mid}");
        }
    }
    //POST 
    public async Task RemoveMoviesByActorPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int amid = int.TryParse(req.QueryString["amid"], out int a) ? a : 1;

        Result<ActorMovie> result = await actorMovieService.Delete(amid);

        if (result.IsValid)
        {
            Console.WriteLine($"DELETING MoviesByActor={amid}");

            HttpUtils.AddOptions(options, "redirect", "message", "ActorMovie removed successfully!");
            await HttpUtils.Redirect(res, req, options, $"/actors/movies?aid={result.Value!.ActorId}");
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
            await HttpUtils.Redirect(res, req, options, $"/actors");
        }
    }
      public async Task RemoveActorsByMoviePost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
     {
        int amid = int.TryParse(req.QueryString["amid"], out int a) ? a : 1;

        Result<ActorMovie> result = await actorMovieService.Delete(amid);

        if (result.IsValid)
        {
            Console.WriteLine($"DELETING RemoveActorsByMovie={amid}");//////////////

            HttpUtils.AddOptions(options, "redirect", "message", "ActorMovie removed successfully!");
            await HttpUtils.Redirect(res, req, options, $"/movies/actors?mid={result.Value!.MovieId}");
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
            await HttpUtils.Redirect(res, req, options, $"/movies");
        }
     }
}
