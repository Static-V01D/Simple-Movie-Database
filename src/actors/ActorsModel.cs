namespace SMDB;
using System.Collections;
using System.Net;
using System.Text;

public class Actor
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Bio { get; set; }
    public float Rating { get; set; } // "admin", "Plus" or "Regular"

    public Actor(int id = 0, string firstName = "", string lastName = "", string bio = "", float rating = 0)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Bio = bio;
        Rating = rating;
    }
    public override string ToString()
    {
        return $"Actor[Id={Id}, FirstName ={FirstName}, LastName={LastName}, Bio={Bio}, Rating={Rating}]";
    }
   
    
}