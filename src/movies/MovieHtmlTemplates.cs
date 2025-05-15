namespace SMDB;

public class MovieHtmlTemplates
{  
        public static string ViewAllMoviesGet(List<Movie> Movies, int MovieCount, int page, int size) 
        {      
         int pageCount = (int)Math.Ceiling((double)MovieCount / size);
            string rows = "";
            foreach (Movie Movie in Movies)
            {
                rows += $@"<tr>
                <td>{Movie.Id}</td>
                <td>{Movie.Title}</td>                
                <td>{Movie.Year}</td>
                <td>{Movie.Description}</td>
                <td>{Movie.Rating}</td>
                <td><a href =""/Movies/view?mid={Movie.Id}"">View</td>
                <td><a href =""/Movies/edit?mid={Movie.Id}"">Edit</td>
                <td>
                <form action =""/Movies/remove?mid={Movie.Id}"" method=""POST"" onsubmit=""return confirm('Are you sure you want to remove this Movie?');"">
                    <input type=""submit"" value=""Remove"">
                </form>
                </td>
                </tr>";
            }

            string html = $@"
            <div class =""add"">
             <a href=""/Movies/add"">Add New Movie</a>
            </div>
            <table class=""viewall"">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Title</th>                       
                        <th>Year</th>
                        <th>Description</th>
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

    public static string AddMovieGet(string title,string year, string description, string rating)
    {


          string html = $@"
        <form class =""addform"" method=""POST"" action=""/Movies/add"">
            <label for=""title"">First Name:</label>
            <input type=""text"" id=""title"" name=""title"" placeholder=""Title"" value =""{title}""><br><br>
            <label for=""Year"">Last Name:</label>
            <input type=""number"" min=""188"" max =""{DateTime.Now.Year}"" step =""1""id=""Year"" name=""Year"" placeholder=""Year"" value=""year""><br><br>
            <label for=""Description"">Last Name:</label>
            <input type=""Description"" id=""Description"" name=""Description"" placeholder=""Description"" value=""{description}""><br><br>
            <label for=""Rating"">Rating:</label>
           <input id=""rating"" name = ""rating"" type=""number"" min=""0"" max = ""10"" step = ""0.1"" values = ""{rating}"">
            <input type=""submit"" value=""Add Movie"">
        </form>       
        ";
        return html;
    }

    public static string ViewMovieGet(Movie Movie)
    {
          string html = $@"           
            <table class =""view"">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Title</th>
                        <th>Rating</th>
                        <th>Year</th>
                        <th>Description</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>{Movie.Id}</td>
                        <td>{Movie.Title}</td>
                        <td>{Movie.Rating}</td>
                        <td>{Movie.Year}</td>
                        <td>{Movie.Description}</td>
                    </tr>
                </tbody>
            </table>             
                      
            ";
            return html;
    }

    public static string EditMovieGet(int mid, Movie Movie)
    {
       

            string html = $@"
            <form  class =""editform""  method=""POST"" action=""/Movies/edit?mid={mid}"">
                <label for=""title"">title:</label>
                <input type=""text"" id=""title"" name=""title"" required value=""{Movie.Title}""><br><br>
                <label for=""Year"">Year:</label>
                 <input type=""number"" min=""1888"" max =""{DateTime.Now.Year}"" step =""1""id=""Year"" name=""Year"" placeholder=""Year"" value=""year""><br><br>
                <label for=""Description"">Last Name:</label>
                <input type=""text"" id=""Description"" name=""Description"" placeholder=""Description"" value=""{Movie.Description}""><br><br>
                <label for=""Rating"">Rating:</label>
               <input id=""rating"" name = ""rating"" type=""number"" min=""0"" max = ""10"" step = ""0.1"" values = ""{Movie.Rating}"">
                <input type=""submit"" value=""Edit Movie"">
            </form>            
            ";
        return html;
    }
}