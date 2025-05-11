namespace SMDB;
using System.Collections;
using System.Net;
using System.Text;

public class AuthenticationController 
{
    private IUserService userService;
    public AuthenticationController(IUserService userService)
    {
        this.userService = userService;
    }     
       
    

    public async Task LandingPageGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string html = $@"
        <nav>
            <head>
                <title>SMDB</title>
                <link rel=""stylesheet"" href=""/css/style.css"">
                <link rel=""icon"" type=""image/x-icon"" href=""/favicon.png"">
                <script type=""text/javascript"" src=""/js/main.js"" defer></script>
                <script type=""text/javascript"" src=""/js/landing.js"" defer></script>
            </head>
            <ul>
                <li><a href=""/register"">Register</a></li>
                <li><a href=""/login"">Login</a></li>
                <li><a href=""/users"">Users</a></li>
                <li><a href=""/logout"">Logout</a></li>
                <li><a href=""/actors"">Actors</a></li>
                <li><a href=""/movies"">Movies</a></li>    
            </ul>
        </nav>
        ";

        string content = HtmlTemplates.Base("SMDB","Landing Page", html);
        
        await HttpUtils.Respond(res, req, options,(int)HttpStatusCode.OK, html);
    }


         
    
        
    
}