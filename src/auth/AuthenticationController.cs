namespace SMDB;
using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Web;

public class AuthenticationController
{
    private IUserService userService;
    public AuthenticationController(IUserService userService)
    {
        this.userService = userService;
    }

    public async Task LandingPageGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string message = req.QueryString["message"] ?? "";
        string html = $@"
        <!DOCTYPE html>
        <html>
        <head>
            <title>SMDB</title>
            <link rel=""stylesheet"" type=""css"" href=""/styles/main.css"">            
            
        </head>
        <body>
            <nav>
                <ul>
                    <li><a href=""/register"">Register</a></li>
                    <li><a href=""/login"">Login</a></li>
                    <li><a href=""/users"">Users</a></li>
                    <li>
                        <form action=""/logout"" method=""POST"">
                           <input type=""submit"" value=""Logout"">
                        </form>
                    </li>
                    <li><a href=""/actors"">Actors</a></li>
                    <li><a href=""/movies"">Movies</a></li>    
                </ul>
            </nav>
        </body>
        </html>
        ";

        string content = HtmlTemplates.Base("SMDB", "Landing Page", html, message);

        await HttpUtils.Respond(res, req, options, (int)HttpStatusCode.OK, content);
    }

    //Get /register

    public async Task RegisterGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var returnUrl = req.QueryString["retrunUrl"] ?? "";
        var rQuery = string.IsNullOrWhiteSpace(returnUrl) ? "" : $"?returnUrl={HttpUtility.UrlEncode(returnUrl)}";
        string message = req.QueryString["message"] ?? "";
        string username = req.QueryString["username"] ?? "";

        string html = $@"
        
            <form action = ""/register{rQuery}"" method=""POST"">
                <label for=""username"">Username</label>
                <input id=""username"" name=""username"" type=""text"" placeholder=""Username"" value=""{username}"">
                <label for=""password"">Password</label>
                <input id=""password"" name=""password"" type=""password"" placeholder=""Password"">
                <label for=""cpassword"">Password</label>
                <input id=""cpassword"" name=""cpassword"" type=""password"" placeholder=""ConfirmPassword"">
                <input type=""submit"" value=""Register"">
            </form>

        
        
        ";

        string content = HtmlTemplates.Base("SMDB", "Register Page", html, message);

        await HttpUtils.Respond(res, req, options, (int)HttpStatusCode.OK, content);
    }

    //POST /register

    public async Task RegisterPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var returnUrl = req.QueryString["returnUrl"] ?? "";

        var formData = (NameValueCollection?)options["req.form"] ?? [];
        var username = formData["username"] ?? "";
        var password = formData["password"] ?? "";
        var cpassword = formData["cpassword"] ?? "";

        if (password != cpassword)
        {
            HttpUtils.AddOptions(options, "redirect", "message", "Passwords do not match");
            HttpUtils.AddOptions(options, "redirect", "username", username);
            HttpUtils.AddOptions(options, "redirect", "returnUrl", returnUrl);

            await HttpUtils.Redirect(res, req, options, $"/register");
        }
        else
        {
            User newUser = new User(0, username, password, "", "");
            var result = await userService.Create(newUser);

            if (result.IsValid)
            {
                HttpUtils.AddOptions(options, "redirect", "message", "User registered succesfully");
                HttpUtils.AddOptions(options, "redirect", "username", username);
                HttpUtils.AddOptions(options, "redirect", "returnUrl", returnUrl);
                await HttpUtils.Redirect(res, req, options, $"/login");
            }
            else
            {
                HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
                HttpUtils.AddOptions(options, "redirect", "username", username);
                HttpUtils.AddOptions(options, "redirect", "returnUrl", returnUrl);
                await HttpUtils.Redirect(res, req, options, $"/register");
            }
        }
    }

    public async Task LoginGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var returnUrl = req.QueryString["retrunUrl"] ?? "";
        var rQuery = string.IsNullOrWhiteSpace(returnUrl) ? "" : $"?returnUrl={HttpUtility.UrlEncode(returnUrl)}";
        string message = req.QueryString["message"] ?? "";
        string username = req.QueryString["username"] ?? "";

        string html = $@"
        
            <form action = ""/login{rQuery}"" method=""POST"">
                <label for=""username"">Username</label>
                <input id=""username"" name=""username"" type=""text"" placeholder=""Username"" value=""{username}"">
                <label for=""password"">Password</label>
                <input id=""password"" name=""password"" type=""password"" placeholder=""Password"">                             
                <input type=""submit"" value=""Login"">
            </form>

        
        
        ";
        string content = HtmlTemplates.Base("SMDB", "Login Page", html, message);

        await HttpUtils.Respond(res, req, options, (int)HttpStatusCode.OK, content);
    }

    public async Task LoginPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var returnUrl = req.QueryString["returnUrl"] ?? "/";

        var formData = (NameValueCollection?)options["req.form"] ?? [];
        var username = formData["username"] ?? "";
        var password = formData["password"] ?? "";

        HttpUtils.AddOptions(options, "redirect", "username", username);

        var result = await userService.GetToken(username, password);

        if (result.IsValid)
        {
            string token = result.Value!;
            HttpUtils.AddOptions(options, "redirect", "message", "User logged in succesfully");
            HttpUtils.AddOptions(options, "redirect", "returnUrl", returnUrl);
            res.SetCookie(new Cookie("token", result.Value!, "/"));
            res.AddHeader("Authorization", $"Bearer {token!}");

            await HttpUtils.Redirect(res, req, options, $"{returnUrl}");
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
            HttpUtils.AddOptions(options, "redirect", "returnUrl", returnUrl);
            await HttpUtils.Redirect(res, req, options, "/login");
        }

    }

    public async Task LogoutPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {

        res.AddHeader("Set-Cookie", "token=; Max-Age=0; Path=/");
        res.AddHeader("WWW-Authenticate", @"Bearer error=""invalid_token"", error_description=""The user logged out.""");

        HttpUtils.AddOptions(options, "redirect", "message", "User logged out succesfully");

        await HttpUtils.Redirect(res, req, options, "/login");
    }

    public async Task Checkauth(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string token = req.Headers["Authorization"]?.Substring(7) ?? req.Cookies["token"]?.Value ?? "";

        var result = await userService.ValidateToken(token);

        if (result.IsValid)
        {
            options["claims"] = result.Value!;
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);

            await HttpUtils.Redirect(res, req, options, "/login");
        }
        await Task.CompletedTask;
    }
    
    public async Task CheckAdmin(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string token = req.Headers["Authorization"]?.Substring(7) ?? req.Cookies["token"]?.Value ?? "";
        
        var result = await userService.ValidateToken(token);

        if (result.IsValid)
        {
            if (result.Value!["role"] == Roles.ADMIN)
            {
                options["claims"] = result.Value!;
            
            }
            else
            {
                HttpUtils.AddOptions(options, "redirect", "message", "Authenticated but not authorized, you need to be admin.");

                await HttpUtils.Redirect(res, req, options, "/login");
            }
           
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);

            await HttpUtils.Redirect(res, req, options, "/login");
        }

        await Task.CompletedTask;
    }
}