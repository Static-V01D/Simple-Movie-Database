namespace SMDB;
using System.Collections;
using System.Net;
using System.Text;

public class AuthenticationController 
{
    public AuthenticationController() 
    {
       
    }

    public async Task LandingPageGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string html = HtmlTemplates.Base("SMDB","Landing Page","Hello World! 2");
        
        await HttpUtils.Respond(res, req, options, html, (int)HttpStatusCode.OK);
    }


         
    
        
    
}