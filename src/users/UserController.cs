using System.Collections;
using System.Net;
using System.Text;
namespace SMDB;

public class UserController
{
    private IUserService userService;
    public UserController(IUserService userService)
    {
        this.userService = userService;
    }
    // /users?page=1&limit=5
    public async Task ViewAllGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {       

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
                rows += $"<tr><td>{user.Id}</td><td>{user.Username}</td><td>{user.Role}</td><td>{user.Password}</td><td>{user.Salt}</td></tr>";
            }

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
            ";

           string content = HtmlTemplates.Base("SMDB","Users View All Page", html);
        
           await HttpUtils.Respond(res, req, options, html, (int)HttpStatusCode.OK);
        }
    }
}