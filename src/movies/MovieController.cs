using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
namespace SMDB;

public class MovieController
{
    private IMovieService MovieService;
    public MovieController(IMovieService MovieService)
    {
        this.MovieService = MovieService;
    }
    // GET/Movies?page=1&limit=5
    public async Task ViewAllMovies(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {       
        string message = req.QueryString["message"] ?? "";
        int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
        int size = int.TryParse(req.QueryString["limit"], out int s) ? s : 5;
        Result<PagedResult<Movie>> result = await MovieService.ReadAll(page, size);

        if(result.IsValid)
        {
            PagedResult<Movie> pagedResult = result.Value!;
            List<Movie> Movies = pagedResult.Values;
            int MovieCount = pagedResult.TotalCount;
           

            string html = MovieHtmlTemplates.ViewAllMoviesGet(Movies, MovieCount, page, size);
            string content = HtmlTemplates.Base("SMDB","Movies View All Page", html, message);      
            
        
           await HttpUtils.Respond(res, req, options, (int)HttpStatusCode.OK, html);
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect","message", result.Error!.Message);
            await HttpUtils.Redirect(res, req, options, "/");
        }
    }
    // GET/Movies/add
    public async Task AddMovieGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string title = req.QueryString["title"] ?? "";
        string year = req.QueryString["Year"] ?? "";    
        string description = req.QueryString["description"] ?? "";                 
        string rating = req.QueryString["rating"] ?? "";
        string message = req.QueryString["message"] ?? "";

        string html = MovieHtmlTemplates.AddMovieGet(title, year, description, rating);
        string content = HtmlTemplates.Base("SMDB","Movie Add Page", html, message);
    
        await HttpUtils.Respond(res, req, options, (int)HttpStatusCode.OK, content);
    }

    // POST/Movies/add/
    public async Task AddMoviePost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var formData = (NameValueCollection?)options["req.form"] ?? [];

        string title = formData["title"] ?? "";
        string description = formData["description"] ?? "";
        int year = int.TryParse(formData["year"], out int y) ? y: DateTime.Now.Year;
        float rating = float.TryParse(formData["rating"], out float r) ? r: 5f;      

        

        Movie newMovie = new Movie(0, title, year, description, rating);
        Result<Movie> result = await MovieService.Create(newMovie);

        if(result.IsValid)
        {
           HttpUtils.AddOptions(options, "redirect", "message", "Movie added successfully!");        

           await HttpUtils.Redirect(res, req, options, "/movies"); //PRG
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect","message", result.Error!.Message);
            HttpUtils.AddOptions(options, "redirect", formData);   

            
            await HttpUtils.Redirect(res, req, options, "/movies/add");
        }
    }

    // GET Movie/{id}

    public async Task ViewMovieGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {       
        string message = req.QueryString["message"] ?? "";
        int mid = int.TryParse(req.QueryString["mid"], out int u) ? u : 1;
      
        Result<Movie> result = await MovieService.Read(mid);

        if(result.IsValid)
        {
            Movie Movie = result.Value!;         
           
          string html = MovieHtmlTemplates.ViewMovieGet(Movie);


           string content = HtmlTemplates.Base("SMDB","Movies View Page", html,message);
        
           await HttpUtils.Respond(res, req, options, (int)HttpStatusCode.OK, html);
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect","message", result.Error!.Message);
            await HttpUtils.Redirect(res, req, options, "/movies");
        }
    }

    // GET Movies/edit?mid={id}
    public async Task EditMovieGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
       string message = req.QueryString["message"] ?? "";

        int mid = int.TryParse(req.QueryString["mid"], out int u) ? u : 1;
      
        Result<Movie> result = await MovieService.Read(mid);

        if(result.IsValid)
        {
            Movie Movie = result.Value!;   
            string html = MovieHtmlTemplates.EditMovieGet(mid,Movie);           
            
            string content = HtmlTemplates.Base("SMDB","Movie Edit Page", html, message);
        
            await HttpUtils.Respond(res, req, options,(int)HttpStatusCode.OK, content);
        }       
         else
        {
            HttpUtils.AddOptions(options, "redirect","message", result.Error!.Message);
            await HttpUtils.Redirect(res, req, options, "/movies");
        }
        
    }

    // EDITPOST/Movies/add/
    public async Task EditMoviePost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int mid = int.TryParse(req.QueryString["mid"], out int u) ? u : 1;
        var formData = (NameValueCollection?)options["req.form"] ?? [];

        string title = formData["title"] ?? "";
        int year = int.TryParse(formData["year"], out int y) ? y: DateTime.Now.Year;
        string description = formData["description"] ?? "";
        float rating= float.TryParse(formData["rating"], out float r) ? r: 5f;
        

        Movie newMovie = new Movie(0, title, year, description, rating);
        Result<Movie> result = await MovieService.Update(mid, newMovie);

        if(result.IsValid)
        {
           HttpUtils.AddOptions(options, "redirect", "message", "Movie updated successfully!");
           await HttpUtils.Redirect(res, req, options, "/movies");
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect","message", result.Error!.Message);            

            await HttpUtils.Redirect(res, req, options, $"/movies/edit?mid={mid}");
        }
    }

    // GET movies/delete?mid={id}

     public async Task RemoveMoviePost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {       
        int mid = int.TryParse(req.QueryString["mid"], out int u) ? u : 1;
      
        Result<Movie> result = await MovieService.Delete(mid);

       if(result.IsValid)
        {
           HttpUtils.AddOptions(options, "redirect", "message", "Movie removed successfully!");
           await HttpUtils.Redirect(res, req, options, "/movies");
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect","message", result.Error!.Message);
            await HttpUtils.Redirect(res, req, options, "/movies/edit");
        }
    }
}
