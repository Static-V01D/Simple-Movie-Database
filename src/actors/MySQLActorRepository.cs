using System.ComponentModel;
using System.Data;
using MySql.Data.MySqlClient;
using Mysqlx.Cursor;
using Org.BouncyCastle.Tls;

namespace SMDB;

public class MySQLActorRepository : IActorRepository
{
    private string connectionString;

    public MySQLActorRepository(string connectionString)
    {
        this.connectionString = connectionString;
      //  Init();
    }

    private void Init()
    {
        using var dbc = OpenDB();

        using var cmd = dbc.CreateCommand();

        cmd.CommandText = @"       
        CREATE TABLE IF NOT EXISTS Actors        
        (
            id int AUTO_INCREMENT PRIMARY KEY,
            firstname NVARCHAR(64) NOT NULL,
            lastname NVARCHAR(64) NOT NULL,
            bio NVARCHAR(4096),
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
    public async Task<PagedResult<Actor>> ReadAll(int page, int Size)
    {
        using var dbc = OpenDB();

        using var countCmd = dbc.CreateCommand();

        countCmd.CommandText = "SELECT COUNT(*) FROM Actors";
        int totalCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

        using var cmd = dbc.CreateCommand();
        cmd.CommandText = "SELECT * FROM Actors LIMIT @offset, @limit";
        cmd.Parameters.AddWithValue("@offset", (page - 1) * Size);
        cmd.Parameters.AddWithValue("@limit", Size);


        using var rows = await cmd.ExecuteReaderAsync();
        var actors = new List<Actor>();
        while (await rows.ReadAsync())
        {
            actors.Add(new Actor
            {
                Id = rows.GetInt32("id"),
                FirstName = rows.GetString("firstname"),
                LastName = rows.GetString("lastname"),
                Bio = rows.GetString("Bio"),
                Rating = rows.GetFloat("rating")

            });

        }
        return new PagedResult<Actor>(actors, totalCount);
    }
    public async Task<Actor?> Create(Actor actor)
    {

        using var dbc = OpenDB();

        using var cmd = dbc.CreateCommand();

        cmd.CommandText = @"
        INSERT INTO Actors (firstname,lastname,bio,rating)
        VALUES(@firstname,@lastname,@bio,@rating);
        SELECT LAST_INSERT_ID();";
        cmd.Parameters.AddWithValue("@firstname", actor.FirstName);
        cmd.Parameters.AddWithValue("@lastname", actor.LastName);
        cmd.Parameters.AddWithValue("@bio", actor.Bio);
        cmd.Parameters.AddWithValue("@rating", actor.Rating);

        actor.Id = Convert.ToInt32(await cmd.ExecuteScalarAsync());

        return actor;
    }


    public async Task<Actor?> Read(int id)
    {

        using var dbc = OpenDB();

        using var cmd = dbc.CreateCommand();

        cmd.CommandText = "SELECT * FROM Actors WHERE id = @id";
        cmd.Parameters.AddWithValue("@id", id);

        using var rows = await cmd.ExecuteReaderAsync();

        if (await rows.ReadAsync())
        {
            return new Actor
            {

                Id = rows.GetInt32("id"),
                FirstName = rows.GetString("firstname"),
                LastName = rows.GetString("lastname"),
                Bio = rows.GetString("Bio"),
                Rating = rows.GetInt32("rating")
            };
        }
        return null;
    }
    public async Task<Actor?> Update(int id, Actor newActor)
    {
        using var dbc = OpenDB();

        using var cmd = dbc.CreateCommand();

        cmd.CommandText = @"
        UPDATE Actors SET
            firstname=@firstname,
            lastname = @lastname,
            bio = @bio,
            rating = @rating
        WHERE id=@id";
        cmd.Parameters.AddWithValue("@firstname", newActor.FirstName);
        cmd.Parameters.AddWithValue("@lastname", newActor.LastName);
        cmd.Parameters.AddWithValue("@bio", newActor.Bio);
        cmd.Parameters.AddWithValue("@rating", newActor.Rating);
        cmd.Parameters.AddWithValue("@id", id);

        return Convert.ToInt32(await cmd.ExecuteNonQueryAsync()) > 0 ? newActor : null;

    }
    public async Task<Actor?> Delete(int id)
    {
        using var dbc = OpenDB();

        using var cmd = dbc.CreateCommand();
        cmd.CommandText = "DELETE FROM Actors Where id = @id";
        cmd.Parameters.AddWithValue("@id", id);

        Actor? actor = await Read(id);

        return Convert.ToInt32(await cmd.ExecuteNonQueryAsync()) > 0 ? actor : null;

    }


}