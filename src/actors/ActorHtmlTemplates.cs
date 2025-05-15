namespace SMDB;

public class ActorHtmlTemplates
{  
        public static string ViewAllActorsGet(List<Actor> Actors, int ActorCount, int page, int size) 
        {      
         int pageCount = (int)Math.Ceiling((double)ActorCount / size);
            string rows = "";
            foreach (Actor Actor in Actors)
            {
                rows += $@"<tr>
                <td>{Actor.Id}</td>
                <td>{Actor.FirstName}</td>                
                <td>{Actor.LastName}</td>
                <td>{Actor.Bio}</td>
                <td>{Actor.Rating}</td>
                <td><a href =""/Actors/view?aid={Actor.Id}"">View</td>
                <td><a href =""/Actors/edit?aid={Actor.Id}"">Edit</td>
                <td>
                <form action =""/Actors/remove?aid={Actor.Id}"" method=""POST"" onsubmit=""return confirm('Are you sure you want to remove this Actor?');"">
                    <input type=""submit"" value=""Remove"">
                </form>
                </td>
                </tr>";
            }

            string html = $@"
            <div class =""add"">
             <a href=""/Actors/add"">Add New Actor</a>
            </div>
            <table class=""viewall"">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>First Name</th>                       
                        <th>Last Name</th>
                        <th>Bio</th>
                        <th>Rating</th>
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

    public static string AddActorGet(string firstName,string lastName, string bio, string rating)
    {


          string html = $@"
        <form class =""addform"" method=""POST"" action=""/Actors/add"">
            <label for=""firstName"">First Name:</label>
            <input type=""text"" id=""firstName"" name=""firstName"" placeholder=""first Name"" value =""{firstName}""><br><br>
            <label for=""LastName"">Last Name:</label>
            <input type=""text"" id=""LastName"" name=""LastName"" placeholder=""Last Name"" value=""lastName""><br><br>
            <label for=""Bio"">Last Name:</label>
            <input type=""Bio"" id=""Bio"" name=""Bio"" placeholder=""Bio"" value=""{bio}""><br><br>
            <label for=""Rating"">Rating:</label>
           <input id=""rating"" name = ""rating"" type=""number"" min=""0"" max = ""10"" step = ""0.1"" values = ""{rating}"">
            <input type=""submit"" value=""Add Actor"">
        </form>       
        ";
        return html;
    }

    public static string ViewActorGet(Actor Actor)
    {
          string html = $@"           
            <table class =""view"">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>First Name</th>
                        <th>Rating</th>
                        <th>Last Name</th>
                        <th>Bio</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>{Actor.Id}</td>
                        <td>{Actor.FirstName}</td>
                        <td>{Actor.Rating}</td>
                        <td>{Actor.LastName}</td>
                        <td>{Actor.Bio}</td>
                    </tr>
                </tbody>
            </table>             
                      
            ";
            return html;
    }

    public static string EditActorGet(int aid, Actor Actor)
    {
       

            string html = $@"
            <form  class =""editform""  method=""POST"" action=""/Actors/edit?aid={aid}"">
                <label for=""firstName"">firstName:</label>
                <input type=""text"" id=""firstName"" name=""firstName"" required value=""{Actor.FirstName}""><br><br>
                <label for=""LastName"">LastName:</label>
                <input type=""text"" id=""LastName"" name=""LastName"" required value=""{Actor.LastName}""><br><br>
                <label for=""Bio"">Last Name:</label>
                <input type=""text"" id=""Bio"" name=""Bio"" placeholder=""Bio"" value=""{Actor.Bio}""><br><br>
                <label for=""Rating"">Rating:</label>
               <input id=""rating"" name = ""rating"" type=""number"" min=""0"" max = ""10"" step = ""0.1"" values = ""{Actor.Rating}"">
                <input type=""submit"" value=""Edit Actor"">
            </form>            
            ";
        return html;
    }
}