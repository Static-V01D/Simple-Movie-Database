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
    public async Task ViewAllGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {       
        string message = req.QueryString["message"] ?? "";
        int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
        int size = int.TryParse(req.QueryString["size"], out int s) ? s : 5;

        Result<PagedResult<User>> result = await userService.ReadAll(page, size);

        if(result.IsValid)
        {
            PagedResult<User> PagedResult = result.Value!;
            List<User> users = PagedResult.Values;
            int userCount = PagedResult.TotalCount;
            int pageCount = (int)Math.Ceiling((double)userCount / size);

            string rows = "";
            foreach (User user in users)
            {
                rows += $@"<tr>
                <td>{user.Id}</td>
                <td>{user.Username}</td>
                <td>{user.Role}</td>
                <td>{user.Password}</td>
                <td>{user.Salt}</td>
                <td><a href =""/users/view?uid={user.Id}"">View</td>
                <td><a href =""/users/edit?uid={user.Id}"">Edit</td>
                <td><a href =""/users/remove?uid={user.Id}"">Remove</td>
                </tr>";
            }

            string html = $@"
            <a href=""/users/add"">Add New User</a>
            <table border=""1"">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Username</th>
                        <th>Role</th>
                        <th>Password</th>
                        <th>Salt</th>
                        <th>View</th>
                        <th>Edit</th>
                        <th>Remove</th>
                    </tr>
                </thead>
                <tbody>
                    {rows}
                </tbody>
            </table>

            <div>                
                <a href=""?page=1&size={size}"">First</a>
                <a href=""?page={page - 1}&size={size}"">Previous</a>
                <span>Page {page} of {pageCount}</span>
                <a href=""?page={page + 1}&size={size}"">Next</a>
                <a href=""?page={pageCount}&size={size}"">Last</a>
            </div>     
            <div>{message}</div>           
            ";

           string content = HtmlTemplates.Base("SMDB","Users View All Page", html);
        
           await HttpUtils.Respond(res, req, options, (int)HttpStatusCode.OK, html);
        }
    }
    // GET/users/add
    public async Task AddGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string username = req.QueryString["username"] ?? "";
        string password = req.QueryString["password"] ?? "";
        string role = req.QueryString["role"] ?? "";
        string message = req.QueryString["message"] ?? "";

        string roles = "";

        foreach (string r in Roles.ROLES)
        {
            string selected = r == role ? " selected" : "";
            roles += $@"<option value=""{r}""{selected}>{r}</option>";
        }
        string html = $@"
        <form method=""POST"" action=""/users/add"">
            <label for=""username"">Username:</label>
            <input type=""text"" id=""username"" name=""username"" placeholder=""Username"" value =""{username}""><br><br>
            <label for=""password"">Password:</label>
            <input type=""password"" id=""password"" name=""password"" placeholder=""Password"" value =""{password}""><br><br>
            <label for=""role"">Role:</label>
            <select id=""role"" name=""role"">
                {roles}
            </select><br><br>
            <input type=""submit"" value=""Add User"">
        </form>
        <div>{message}</div>
        ";

        string content = HtmlTemplates.Base("SMDB","User Add Page", html);
    
        await HttpUtils.Respond(res, req, options, (int)HttpStatusCode.OK, html);
    }

    // POST/users/add/
    public async Task AddPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
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

           await HttpUtils.Redirect(res, req, options, "/users");
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", formData);
            await HttpUtils.Redirect(res, req, options, $"/users/add?username={Uri.EscapeDataString(username)}&password={Uri.EscapeDataString(password)}&role={Uri.EscapeDataString(role)}&message={Uri.EscapeDataString(result.Error!.Message)}");
        }
    }

    // GET user/{id}

    public async Task ViewGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {       
        string message = req.QueryString["message"] ?? "";
        int uid = int.TryParse(req.QueryString["uid"], out int u) ? u : 1;
      
        Result<User> result = await userService.Read(uid);

        if(result.IsValid)
        {
            User user = result.Value!;         
           
            string html = $@"           
            <table border=""1"">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Username</th>
                        <th>Role</th>
                        <th>Password</th>
                        <th>Salt</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>{user.Id}</td>
                        <td>{user.Username}</td>
                        <td>{user.Role}</td>
                        <td>{user.Password}</td>
                        <td>{user.Salt}</td>
                    </tr>
                </tbody>
            </table>
               
            <div>Total Users: {message}</div>           
            ";

           string content = HtmlTemplates.Base("SMDB","Users View Page", html);
        
           await HttpUtils.Respond(res, req, options, (int)HttpStatusCode.OK, html);
        }
    }

    // GET users/edit?uid={id}
    public async Task EditGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
       string message = req.QueryString["message"] ?? "";

        int uid = int.TryParse(req.QueryString["uid"], out int u) ? u : 1;
      
        Result<User> result = await userService.Read(uid);

        if(result.IsValid)
        {
            User user = result.Value!;   

            
            string roles = "";

            foreach (string role in Roles.ROLES)
            {
                string selected = (role == user.Role) ? "selected" : "";
                roles += $@"<option value=""{role}""{selected}>{role}</option>";
            }
            string html = $@"
            <form method=""POST"" action=""/users/edit?uid={uid}"">
                <label for=""username"">Username:</label>
                <input type=""text"" id=""username"" name=""username"" required value=""{user.Username}""><br><br>
                <label for=""password"">Password:</label>
                <input type=""password"" id=""password"" name=""password"" required value=""{user.Password}""><br><br>
                <label for=""role"">Role:</label>
                <select id=""role"" name=""role"">
                    {roles}
                </select><br><br>
                <input type=""submit"" value=""Edit User"">
            </form>
            <div>{message}</div>
            ";

            string content = HtmlTemplates.Base("SMDB","User Edit Page", html);
        
            await HttpUtils.Respond(res, req, options,(int)HttpStatusCode.OK, html);
        } 
        //You can add an else statement here to handle the case when the user is not found. AND ADD A PAGE THAT IS COOLER
        
        
    }

    // EDITPOST/users/add/
    public async Task EditPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
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
            options["message"] = "User edited successfully!";
           await HttpUtils.Redirect(res, req, options, "/users");
        }
        else
        {
            options["message"] = result.Error!.Message;
            await HttpUtils.Redirect(res, req, options, "/users/edit");
        }
    }

    // GET users/delete?uid={id}

     public async Task RemoveGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {       
        int uid = int.TryParse(req.QueryString["uid"], out int u) ? u : 1;
      
        Result<User> result = await userService.Delete(uid);

         if(result.IsValid)
        {
            options["message"] = "User removed successfully!";
           await HttpUtils.Redirect(res, req, options, "/users");
        }
        else
        {
            options["message"] = result.Error!.Message;
            await HttpUtils.Redirect(res, req, options, "/users");
        }
    }
}
