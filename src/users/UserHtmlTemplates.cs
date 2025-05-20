
namespace SMDB;

public class UserHtmlTemplates
{  
        public static string ViewAllUsersGet(List<User> Users, int UserCount, int page, int size) 
        {      
         int pageCount = (int)Math.Ceiling((double)UserCount / size);
            string rows = "";
            foreach (User User in Users)
            {
                rows += $@"<tr>
                <td>{User.Id}</td>
                <td>{User.Username}</td>
                <td>{User.Role}</td>
                <td>{User.Password}</td>
                <td>{User.Salt}</td>
                <td><a href =""/users/view?uid={User.Id}"">View</td>
                <td><a href =""/users/edit?uid={User.Id}"">Edit</td>
                <td>
                <form action =""/users/remove?uid={User.Id}"" method=""POST"" onsubmit=""return confirm('Are you sure you want to remove this User?');"">
                    <input type=""submit"" value=""Remove"">
                </form>
                </td>
                </tr>";
            }

            string html = $@"
            <div class =""add"">
             <a href=""/users/add"">Add New User</a>
            </div>
            <table class=""viewall"">
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

            <div class =""pagination"">                
                <a href=""?page=1&size={size}"">First</a>
                <a href=""?page={page - 1}&size={size}"">Previous</a>
                <span>Page {page} of {pageCount}</span>
                <a href=""?page={page + 1}&size={size}"">Next</a>
                <a href=""?page={pageCount}&size={size}"">Last</a>
            </div>    
              
            ";
          return html; 
    }

    public static string AddUserGet(string Username, string role)
    {
        string roles = "";

        foreach (string r in Roles.ROLES)
        {
            string selected = r == role ? " selected" : "";
            roles += $@"<option value=""{r}""{selected}>{r}</option>";
        }

          string html = $@"
        <form class =""addform"" method=""POST"" action=""/users/add"">
            <label for=""Username"">Username:</label>
            <input type=""text"" id=""Username"" name=""Username"" placeholder=""Username"" value =""{Username}""><br><br>
            <label for=""password"">Password:</label>
            <input type=""password"" id=""password"" name=""password"" placeholder=""Password""><br><br>
            <label for=""role"">Role:</label>
            <select id=""role"" name=""role"">
                {roles}
            </select><br><br>
            <input type=""submit"" value=""Add User"">
        </form>       
        ";
        return html;
    }

    public static string ViewUserGet(User User)
    {
          string html = $@"           
            <table class =""view"">
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
                        <td>{User.Id}</td>
                        <td>{User.Username}</td>
                        <td>{User.Role}</td>
                        <td>{User.Password}</td>
                        <td>{User.Salt}</td>
                    </tr>
                </tbody>
            </table>             
                      
            ";
            return html;
    }

    public static string EditUserGet(int uid, User User)
    {
        string roles = "";

            foreach (string role in Roles.ROLES)
            {
                string selected = (role == User.Role) ? "selected" : "";
                roles += $@"<option value=""{role}""{selected}>{role}</option>";
            }
            string html = $@"
            <form  class =""editform""  method=""POST"" action=""/users/edit?uid={uid}"">
                <label for=""Username"">Username:</label>
                <input type=""text"" id=""Username"" name=""Username"" required value=""{User.Username}""><br><br>
                <label for=""password"">Password:</label>
                <input type=""password"" id=""password"" name=""password"" required value=""{User.Password}""><br><br>
                <label for=""role"">Role:</label>
                <select id=""role"" name=""role"">
                    {roles}
                </select><br><br>
                <input type=""submit"" value=""Edit User"">
            </form>            
            ";
        return html;
    }
}