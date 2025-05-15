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
        <!DOCTYPE html>
        <html>
        <head>
            <title>SMDB</title>
            <link rel=""stylesheet"" type=""css"" href=""styles/main.css"">
            <link rel=""icon"" type=""image/x-icon"" href=""/favicon.png"">
           
        </head>
        <body>
            <nav>
                <ul>
                    <li><a href=""/register"">Register</a></li>
                    <li><a href=""/login"">Login</a></li>
                    <li><a href=""/users"">Users</a></li>
                    <li><a href=""/logout"">Logout</a></li>
                    <li><a href=""/Actors"">Actors</a></li>
                    <li><a href=""/Movies"">Movies</a></li>    
                </ul>
            </nav>
        </body>
        </html>
        ";

        string content = HtmlTemplates.Base("SMDB", "Landing Page", html);

        await HttpUtils.Respond(res, req, options, (int)HttpStatusCode.OK, content);
    }
}