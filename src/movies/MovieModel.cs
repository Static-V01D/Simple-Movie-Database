namespace SMDB;
using System.Collections;
using System.Net;
using System.Text;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int Year { get; set; }
    public string Description { get; set; }
    public float Rating { get; set; } // "admin", "Plus" or "Regular"

    public Movie(int id = 0, string title = "", int year = 2025, string description = "", float rating = 0)
    {
        Id = id;
        Title = title;
        Year = year;
        Description = description;
        Rating = rating;
    }
    public override string ToString()
    {
        return $"Movie[Id={Id}, Title ={Title}, Year={Year}, Description={Description}, Rating={Rating}]";
    }
   
    
}