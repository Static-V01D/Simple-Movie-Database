using System.ComponentModel;
using System.Data;
using MySql.Data.MySqlClient;
using Mysqlx.Cursor;
using Org.BouncyCastle.Tls;

namespace SMDB;

public class MySQLUserRepository : IUserRepository
{
    private string connectionString;

    public MySQLUserRepository(string connectionString)
    {
        this.connectionString = connectionString;
        //  Init();
    }

    private void Init()
    {
        using var dbc = OpenDB();

        using var cmd = dbc.CreateCommand();

        cmd.CommandText = @"       
        CREATE TABLE IF NOT EXISTS Users        
        (
            id int AUTO_INCREMENT PRIMARY KEY,
            username NVARCHAR(64) NOT NULL UNIQUE,
            password NVARCHAR(64) NOT NULL,
            salt NVARCHAR(4096) NOT NULL,
            role ENUM('Admin','Plus') NOT NULL
          
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
    public async Task<PagedResult<User>> ReadAll(int page, int Size)
    {
        using var dbc = OpenDB();

        using var countCmd = dbc.CreateCommand();

        countCmd.CommandText = "SELECT COUNT(*) FROM Users";
        int totalCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

        using var cmd = dbc.CreateCommand();
        cmd.CommandText = "SELECT * FROM Users LIMIT @offset, @limit";
        cmd.Parameters.AddWithValue("@offset", (page - 1) * Size);
        cmd.Parameters.AddWithValue("@limit", Size);


        using var rows = await cmd.ExecuteReaderAsync();
        var users = new List<User>();
        while (await rows.ReadAsync())
        {
            users.Add(new User
            {
                Id = rows.GetInt32("id"),
                Username = rows.GetString("username"),
                Password = rows.GetString("password"),
                Salt = rows.GetString("salt"),
                Role = rows.GetString("role")

            });

        }
        return new PagedResult<User>(users, totalCount);
    }
    public async Task<User?> Create(User user)
    {

        using var dbc = OpenDB();

        using var cmd = dbc.CreateCommand();

        cmd.CommandText = @"
        INSERT INTO Users (username,password,salt,role)
        VALUES(@username,@password,@salt,@role);
        SELECT LAST_INSERT_ID();";
        cmd.Parameters.AddWithValue("@username", user.Username);
        cmd.Parameters.AddWithValue("@password", user.Password);
        cmd.Parameters.AddWithValue("@salt", user.Salt);
        cmd.Parameters.AddWithValue("@role", user.Role);

        user.Id = Convert.ToInt32(await cmd.ExecuteScalarAsync());

        return user;
    }


    public async Task<User?> Read(int id)
    {

        using var dbc = OpenDB();

        using var cmd = dbc.CreateCommand();

        cmd.CommandText = "SELECT * FROM Users WHERE id = @id";
        cmd.Parameters.AddWithValue("@id", id);

        using var rows = await cmd.ExecuteReaderAsync();

        if (await rows.ReadAsync())
        {
            return new User
            {

                Id = rows.GetInt32("id"),
                Username = rows.GetString("username"),
                Password = rows.GetString("password"),
                Salt = rows.GetString("salt"),
                Role = rows.GetString("role")
            };
        }
        return null;
    }
    public async Task<User?> Update(int id, User newUser)
    {
        using var dbc = OpenDB();

        using var cmd = dbc.CreateCommand();

        cmd.CommandText = @"
        UPDATE Users SET
            username=@username,
            password = @password,
            salt = @salt,
            role = @role
        WHERE id=@id";
        cmd.Parameters.AddWithValue("@username", newUser.Username);
        cmd.Parameters.AddWithValue("@password", newUser.Password);
        cmd.Parameters.AddWithValue("@salt", newUser.Salt);
        cmd.Parameters.AddWithValue("@role", newUser.Role);
        cmd.Parameters.AddWithValue("@id", id);

        return Convert.ToInt32(await cmd.ExecuteNonQueryAsync()) > 0 ? newUser : null;

    }
    public async Task<User?> Delete(int id)
    {
        using var dbc = OpenDB();

        using var cmd = dbc.CreateCommand();
        cmd.CommandText = "DELETE FROM Users Where id = @id";
        cmd.Parameters.AddWithValue("@id", id);

        User? user = await Read(id);

        return Convert.ToInt32(await cmd.ExecuteNonQueryAsync()) > 0 ? user : null;

    }
    public async Task<User?> GetUserByUsername(string username)
    {
        using var dbc = OpenDB();

        using var cmd = dbc.CreateCommand();

        cmd.CommandText = "SELECT * FROM Users WHERE username = @username";
        cmd.Parameters.AddWithValue("@username", username);

        using var rows = await cmd.ExecuteReaderAsync();

        if (await rows.ReadAsync())
        {
            return new User
            {

                Id = rows.GetInt32("id"),
                Username = rows.GetString("username"),
                Password = rows.GetString("password"),
                Salt = rows.GetString("salt"),
                Role = rows.GetString("role")
            };
        }
        return null;
    }

}