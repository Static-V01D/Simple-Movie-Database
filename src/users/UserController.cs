using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
namespace SMDB;

public class UserController
{
    private IUserService userService;
    public UserController(IUserService userService)
    {
        this.userService = userService;
    }
    // GET/users?page=1&limit=5
    public async Task ViewAllUsers(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {       
        string message = req.QueryString["message"] ?? "";
        int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
        int size = int.TryParse(req.QueryString["limit"], out int s) ? s : 5;
        Result<PagedResult<User>> result = await userService.ReadAll(page, size);

        if(result.IsValid)
        {
            PagedResult<User> pagedResult = result.Value!;
            List<User> users = pagedResult.Values;
            int userCount = pagedResult.TotalCount;
           

            string html = UserHtmlTemplates.ViewAllUsersGet(users, userCount, page, size);
            string content = HtmlTemplates.Base("SMDB","Users View All Page", html, message);      
            
        
           await HttpUtils.Respond(res, req, options, (int)HttpStatusCode.OK, html);
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect","message", result.Error!.Message);
            await HttpUtils.Redirect(res, req, options, "/");
        }
    }
    // GET/users/add
    public async Task AddUserGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string username = req.QueryString["username"] ?? "";       
        string role = req.QueryString["role"] ?? "";
        string message = req.QueryString["message"] ?? "";

        string html = UserHtmlTemplates.AddUserGet(username, role);
        string content = HtmlTemplates.Base("SMDB","User Add Page", html, message);
    
        await HttpUtils.Respond(res, req, options, (int)HttpStatusCode.OK, content);
    }

    // POST/users/add/
    public async Task AddUserPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var formData = (NameValueCollection?)options["req.form"] ?? [];

        string username = formData["username"] ?? "";
        string password = formData["password"] ?? "";
        string role = formData["role"] ?? "";      

        

        User newUser = new User(0, username, password, "", role);
        Result<User> result = await userService.Create(newUser);

        if(result.IsValid)
        {
           HttpUtils.AddOptions(options, "redirect", "message", "User added successfully!");        

           await HttpUtils.Redirect(res, req, options, "/users"); //PRG
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect","message", result.Error!.Message);
            HttpUtils.AddOptions(options, "redirect","username", formData["username"]);
            HttpUtils.AddOptions(options, "redirect","role", formData["role"]);

            
            await HttpUtils.Redirect(res, req, options, "/users/add");
        }
    }

    // GET user/{id}

    public async Task ViewUserGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {       
        string message = req.QueryString["message"] ?? "";
        int uid = int.TryParse(req.QueryString["uid"], out int u) ? u : 1;
      
        Result<User> result = await userService.Read(uid);

        if(result.IsValid)
        {
            User user = result.Value!;         
           
          string html = UserHtmlTemplates.ViewUserGet(user);


           string content = HtmlTemplates.Base("SMDB","Users View Page", html,message);
        
           await HttpUtils.Respond(res, req, options, (int)HttpStatusCode.OK, html);
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect","message", result.Error!.Message);
            await HttpUtils.Redirect(res, req, options, "/users");
        }
    }

    // GET users/edit?uid={id}
    public async Task EditUserGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
       string message = req.QueryString["message"] ?? "";

        int uid = int.TryParse(req.QueryString["uid"], out int u) ? u : 1;
      
        Result<User> result = await userService.Read(uid);

        if(result.IsValid)
        {
            User user = result.Value!;   
            string html = UserHtmlTemplates.EditUserGet(uid,user);           
            
            string content = HtmlTemplates.Base("SMDB","User Edit Page", html, message);
        
            await HttpUtils.Respond(res, req, options,(int)HttpStatusCode.OK, content);
        }       
         else
        {
            HttpUtils.AddOptions(options, "redirect","message", result.Error!.Message);
            await HttpUtils.Redirect(res, req, options, "/users");
        }
        
    }

    // EDITPOST/users/add/
    public async Task EditUserPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int uid = int.TryParse(req.QueryString["uid"], out int u) ? u : 1;
        var formData = (NameValueCollection?)options["req.form"] ?? [];

        string username = formData["username"] ?? "";
        string password = formData["password"] ?? "";
        string role = formData["role"] ?? "";      

        

        User newUser = new User(0, username, password, "", role);
        Result<User> result = await userService.Update(uid, newUser);

        if(result.IsValid)
        {
           HttpUtils.AddOptions(options, "redirect", "message", "User updated successfully!");
           await HttpUtils.Redirect(res, req, options, "/users");
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect","message", result.Error!.Message);
            await HttpUtils.Redirect(res, req, options, $"/users/edit?uid={uid}");
        }
    }

    // GET users/delete?uid={id}

     public async Task RemoveUserPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {       
        int uid = int.TryParse(req.QueryString["uid"], out int u) ? u : 1;
      
        Result<User> result = await userService.Delete(uid);

       if(result.IsValid)
        {
           HttpUtils.AddOptions(options, "redirect", "message", "User removed successfully!");
           await HttpUtils.Redirect(res, req, options, "/users");
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect","message", result.Error!.Message);
            await HttpUtils.Redirect(res, req, options, "/users/edit");
        }
    }
}
