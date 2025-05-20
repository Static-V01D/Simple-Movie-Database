using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
namespace SMDB;

public class ActorController
{
    private IActorService ActorService;
    public ActorController(IActorService ActorService)
    {
        this.ActorService = ActorService;
    }
    // GET/Actors?page=1&limit=5
    public async Task ViewAllActors(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {       
        string message = req.QueryString["message"] ?? "";
        int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
        int size = int.TryParse(req.QueryString["limit"], out int s) ? s : 5;
        Result<PagedResult<Actor>> result = await ActorService.ReadAll(page, size);

        if(result.IsValid)
        {
            PagedResult<Actor> pagedResult = result.Value!;
            List<Actor> Actors = pagedResult.Values;
            int ActorCount = pagedResult.TotalCount;
           

            string html = ActorHtmlTemplates.ViewAllActorsGet(Actors, ActorCount, page, size);
            string content = HtmlTemplates.Base("SMDB","Actors View All Page", html, message);      
            
        
           await HttpUtils.Respond(res, req, options, (int)HttpStatusCode.OK, html);
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect","message", result.Error!.Message);
            await HttpUtils.Redirect(res, req, options, "/");
        }
    }
    // GET/Actors/add
    public async Task AddActorGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string firstName = req.QueryString["firstName"] ?? "";
        string lastName = req.QueryString["LastName"] ?? "";    
        string bio = req.QueryString["bio"] ?? "";                 
        string rating = req.QueryString["rating"] ?? "";
        string message = req.QueryString["message"] ?? "";

        string html = ActorHtmlTemplates.AddActorGet(firstName, lastName, bio, rating);
        string content = HtmlTemplates.Base("SMDB","Actor Add Page", html, message);
    
        await HttpUtils.Respond(res, req, options, (int)HttpStatusCode.OK, content);
    }

    // POST/Actors/add/
    public async Task AddActorPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var formData = (NameValueCollection?)options["req.form"] ?? [];

        string firstName = formData["firstName"] ?? "";
        string bio = formData["bio"] ?? "";
        string lastname = formData["lastname"] ?? "";
        float rating = float.TryParse(formData["rating"], out float r) ? r: 5f;      

        

        Actor newActor = new Actor(0, firstName, lastname, bio, rating);
        Result<Actor> result = await ActorService.Create(newActor);

        if(result.IsValid)
        {
           HttpUtils.AddOptions(options, "redirect", "message", "Actor added successfully!");        

           await HttpUtils.Redirect(res, req, options, "/actors"); //PRG
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect","message", result.Error!.Message);
            HttpUtils.AddOptions(options, "redirect", formData);   

            
            await HttpUtils.Redirect(res, req, options, "/actors/add");
        }
    }

    // GET Actor/{id}

    public async Task ViewActorGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {       
        string message = req.QueryString["message"] ?? "";
        int aid = int.TryParse(req.QueryString["aid"], out int u) ? u : 1;
      
        Result<Actor> result = await ActorService.Read(aid);

        if(result.IsValid)
        {
            Actor Actor = result.Value!;         
           
          string html = ActorHtmlTemplates.ViewActorGet(Actor);


           string content = HtmlTemplates.Base("SMDB","Actors View Page", html,message);
        
           await HttpUtils.Respond(res, req, options, (int)HttpStatusCode.OK, html);
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect","message", result.Error!.Message);
            await HttpUtils.Redirect(res, req, options, "/actors");
        }
    }

    // GET Actors/edit?aid={id}
    public async Task EditActorGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
       string message = req.QueryString["message"] ?? "";

        int aid = int.TryParse(req.QueryString["aid"], out int u) ? u : 1;
      
        Result<Actor> result = await ActorService.Read(aid);

        if(result.IsValid)
        {
            Actor Actor = result.Value!;   
            string html = ActorHtmlTemplates.EditActorGet(aid,Actor);           
            
            string content = HtmlTemplates.Base("SMDB","Actor Edit Page", html, message);
        
            await HttpUtils.Respond(res, req, options,(int)HttpStatusCode.OK, content);
        }       
         else
        {
            HttpUtils.AddOptions(options, "redirect","message", result.Error!.Message);
            await HttpUtils.Redirect(res, req, options, "/actors");
        }
        
    }

    // EDITPOST/Actors/add/
    public async Task EditActorPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int aid = int.TryParse(req.QueryString["aid"], out int u) ? u : 1;
        var formData = (NameValueCollection?)options["req.form"] ?? [];

        string firstName = formData["firstName"] ?? "";
        string lastname = formData["lastname"] ?? "";
        string bio = formData["bio"] ?? "";
        float rating= float.TryParse(formData["rating"], out float r) ? r: 5f;
        

        Actor newActor = new Actor(0, firstName, lastname, bio, rating);
        Result<Actor> result = await ActorService.Update(aid, newActor);

        if(result.IsValid)
        {
           HttpUtils.AddOptions(options, "redirect", "message", "Actor updated successfully!");
           await HttpUtils.Redirect(res, req, options, "/actors");
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect","message", result.Error!.Message);            

            await HttpUtils.Redirect(res, req, options, $"/actors/edit?aid={aid}");
        }
    }

    // GET Actors/delete?aid={id}

     public async Task RemoveActorPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {       
        int aid = int.TryParse(req.QueryString["aid"], out int u) ? u : 1;
      
        Result<Actor> result = await ActorService.Delete(aid);

       if(result.IsValid)
        {
           HttpUtils.AddOptions(options, "redirect", "message", "Actor removed successfully!");
           await HttpUtils.Redirect(res, req, options, "/actors");
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect","message", result.Error!.Message);
            await HttpUtils.Redirect(res, req, options, "/actors/edit");
        }
    }
}
