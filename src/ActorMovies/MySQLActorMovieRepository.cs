using System.ComponentModel;
using System.Data;
using MySql.Data.MySqlClient;
using Mysqlx.Cursor;
using Org.BouncyCastle.Tls;

namespace SMDB;

public class MySQLActorMovieRepository : IActorMovieRepository
{
    private string connectionString;

    public MySQLActorMovieRepository(string connectionString)
    {
        this.connectionString = connectionString;
          Init();
    }

    private void Init()
    {
        using var dbc = OpenDB();

        using var cmd = dbc.CreateCommand();

        cmd.CommandText = @"       
        CREATE TABLE IF NOT EXISTS ActorsMovies        
        (
            id int AUTO_INCREMENT PRIMARY KEY,
            actorId int NOT NULL,
            movieId int NOT NULL,
            rolename NVARCHAR(64),
            FOREIGN KEY(actorId) REFERENCES Actors(id) ON DELETE CASCADE,
            FOREIGN KEY(movieId) REFERENCES Movie(id) ON DELETE CASCADE,
            rating float
        )            
        ";

        cmd.ExecuteNonQuery();

    }
    public MySqlConnection OpenDB()
    {
        var dbc = new MySqlConnection(connectionString);
        dbc.Open();
        return dbc;
    }

    public async Task<PagedResult<(ActorMovie, Movie)>> ReadAllMoviesByActor(int actorId, int page, int size)
    {
        using var dbc = OpenDB();

        using var cmd = dbc.CreateCommand();

        using var countCmd = dbc.CreateCommand();
        countCmd.CommandText = @"

        SELECT COUNT(*) FROM ActorsMovies as am   
        JOIN MOVIES as m ON am.movieId = m.id 
        WHERE am.actorId = @actorId       
        ";
        countCmd.Parameters.AddWithValue("@actorId", actorId);
        int totalCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

        cmd.CommandText = @"
        SELECT * FROM ActorsMovies as am   
        JOIN MOVIES as m ON am.movieId = m.id 
        WHERE am.actorId = @actorId   
        LIMIT @offset,@limit     
        ";
        cmd.Parameters.AddWithValue("@actorId", actorId);
        cmd.Parameters.AddWithValue("@offset", (page - 1) * size);
        cmd.Parameters.AddWithValue("@limit", size);

        using var rows = await cmd.ExecuteReaderAsync();

        var amms = new List<(ActorMovie, Movie)>();
        while (await rows.ReadAsync())
        {
            ActorMovie am = new ActorMovie(
            rows.GetInt32(0), //id
             rows.GetInt32(1), //actorId
             rows.GetInt32(2), //movieId
             rows.GetString(3)); //rolename
            Movie m = new Movie(
              rows.GetInt32(4), //id
              rows.GetString(5),  //title
              rows.GetInt32(6),  //Year
              rows.GetString(7),  //description
              rows.GetFloat(8)  //rating
              );
            amms.Add((am, m));
        }
        return new PagedResult<(ActorMovie, Movie)>(amms, totalCount);
    }
    public async Task<PagedResult<(ActorMovie, Actor)>> ReadAllActorsByMovie(int movieId, int page, int size)
    {
        using var dbc = OpenDB();

        using var cmd = dbc.CreateCommand();

        using var countCmd = dbc.CreateCommand();
        countCmd.CommandText = @"

        SELECT COUNT(*) FROM ActorsMovies as am   
        JOIN Actors as a ON am.actorId = a.id 
        WHERE am.movieId = @movieId       
        ";
        countCmd.Parameters.AddWithValue("@movieId", movieId);

        int totalCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

        cmd.CommandText = @"
        SELECT * FROM ActorsMovies as am   
        JOIN Actors as a ON am.actorId = a.id 
        WHERE am.movieId = @movieId  
        LIMIT @offset,@limit    
        ";
        cmd.Parameters.AddWithValue("@movieId", movieId);
        cmd.Parameters.AddWithValue("@offset", (page - 1) * size);
        cmd.Parameters.AddWithValue("@limit", size);

        using var rows = await cmd.ExecuteReaderAsync();

        var amas = new List<(ActorMovie, Actor)>();
        while (await rows.ReadAsync())
        {
            ActorMovie am = new ActorMovie(
            rows.GetInt32(0), //id
             rows.GetInt32(1), //actorId
             rows.GetInt32(2), //movieId
             rows.GetString(3)); //rolename
            Actor a = new Actor(
              rows.GetInt32(4), //id
              rows.GetString(5),  //firstname
              rows.GetString(6),  //lastname
              rows.GetString(7),  //bio
              rows.GetFloat(8)  //rating
              );
            amas.Add((am, a));
        }
        return new PagedResult<(ActorMovie, Actor)>(amas, totalCount);
    }
    public async Task<List<Actor>> ReadAllActors()
    {
        using var dbc = OpenDB();

        using var cmd = dbc.CreateCommand();

        using var countCmd = dbc.CreateCommand();
        countCmd.CommandText = @"

        SELECT COUNT(*) FROM Actors    
        ";

        int totalCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

        cmd.CommandText = @"
        SELECT * FROM Actors      
        ";

        using var rows = await cmd.ExecuteReaderAsync();

        var actors = new List<Actor>();
        while (await rows.ReadAsync())
        {
            Actor a = new Actor(
            rows.GetInt32(0), //id
            rows.GetString(1), //firstName
            rows.GetString(2), //lastName
            rows.GetString(3), //Bio
            rows.GetFloat(4)); //rating

            actors.Add(a);
        }
        return actors;
    }
    public async Task<List<Movie>> ReadAllMovies()
    {
        using var dbc = OpenDB();

        using var cmd = dbc.CreateCommand();

        using var countCmd = dbc.CreateCommand();
        countCmd.CommandText = @"

        SELECT COUNT(*) FROM Movies    
        ";

        int totalCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

        cmd.CommandText = @"
        SELECT * FROM Movies      
        ";

        using var rows = await cmd.ExecuteReaderAsync();

        var movie = new List<Movie>();
        while (await rows.ReadAsync())
        {
            Movie a = new Movie(
            rows.GetInt32(0), //id
            rows.GetString(1), //Title
            rows.GetInt32(2), //Year
            rows.GetString(3), //description
            rows.GetFloat(4)); //rating

            movie.Add(a);
        }
        return movie;
    }
    public async Task<ActorMovie> create(int actorId, int movieId, string roleName)
    {
        using var dbc = OpenDB();
        Console.WriteLine($"Inserting actorId={actorId}, movieId={movieId}");
        using var cmd = dbc.CreateCommand();
        cmd.CommandText = @"
        INSERT INTO ActorsMovies (actorId, movieId, rolename)
        VALUES (@actorId, @movieId, @rolename);
        ";
        cmd.Parameters.AddWithValue("@actorId", actorId);
        cmd.Parameters.AddWithValue("@movieId", movieId);
        cmd.Parameters.AddWithValue("@rolename", roleName);

        await cmd.ExecuteNonQueryAsync();

        // Now get the last inserted id
        cmd.CommandText = "SELECT LAST_INSERT_ID();";
        cmd.Parameters.Clear();
        int id = Convert.ToInt32(await cmd.ExecuteScalarAsync());

        var actorMovie = new ActorMovie(id, actorId, movieId, roleName);

        return actorMovie;
    }
    public async Task<ActorMovie> Delete(int id)
    {
        using var dbc = OpenDB();

        using var cmd = dbc.CreateCommand();
        cmd.CommandText = @"
        DELETE FROM ActorsMovies WHERE id = @id;
        ";
        cmd.Parameters.AddWithValue("@id", id);

        var actorMovie = await Read(id);

       return (await cmd.ExecuteNonQueryAsync() > 0) ? actorMovie : null;
        
    }

    public async Task<ActorMovie> Read(int id)
    {
        using var dbc = OpenDB();

        using var cmd = dbc.CreateCommand();

        cmd.CommandText = @"
        SELECT * FROM ActorsMovies WHERE id = @id      
        ";
        cmd.Parameters.AddWithValue("@id", id);


        using var rows = await cmd.ExecuteReaderAsync();

        
        if (await rows.ReadAsync())
        {
            return new ActorMovie(
            rows.GetInt32(0), //id
            rows.GetInt32(1), //actorId
            rows.GetInt32(2), //movieId
            rows.GetString(3)); //rolename


        }
        return null;
    }
}
