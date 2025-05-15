namespace SMDB;

public class ActorMovie
{
    public int Id {get; set;}
    public int ActorId {get; set;}

    public int MovieId {get;set;}
    public string Rolename {get;set;}

    public ActorMovie(int id, int actorId, int movieId, string roleName )
    {
        Id = id;
        ActorId = actorId;
        MovieId = movieId;
        Rolename = roleName; 
    }

    public override string ToString()
    {
        return $"ActorMovie[Id={Id}, ActorId = {ActorId}, MovieId = {MovieId}, RoleName ={Rolename}]";
    }
}