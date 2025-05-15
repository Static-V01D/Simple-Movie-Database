namespace SMDB;
using System.Collections;
using System.Net;
using System.Text;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Salt { get; set; }
    public string Role { get; set; } // "admin", "Plus" or "Regular"

    public User(int id = 0, string username = "", string password = "", string salt = "", string role = "Regular")
    {
        Id = id;
        Username = username;
        Password = password;
        Salt = salt;
        Role = role;
    }   
    public override string ToString()
    {
        return $"User[Id={Id}, Username ={Username}, Password={Password}, Salt={Salt}, Role={Role}]";
    }
    
}