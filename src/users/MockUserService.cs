using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace SMDB;

public class MockUserService : IUserService
{
    private IUserRepository userRepository;

    public MockUserService(IUserRepository userRepository)
    {
        this.userRepository = userRepository;

      //  _ = Create(new User(0, "Admin", "masteroftheuniverse", "", Roles.ADMIN)); //////////////Comment after first run
    }

    public async Task<Result<PagedResult<User>>> ReadAll(int page, int Size)
    {
        var PagedResult = await userRepository.ReadAll(page, Size);

        var result = (PagedResult == null) ?
            new Result<PagedResult<User>>(new Exception("No Results Found")) :
            new Result<PagedResult<User>>(PagedResult);
        return await Task.FromResult(result);
    }
    public async Task<Result<User>> Create(User newUser)
    {
        if (string.IsNullOrWhiteSpace(newUser.Role))
        {
            newUser.Role = Roles.USER;
        }


        if (string.IsNullOrWhiteSpace(newUser.Username))
        {
            return new Result<User>(new Exception("User name cannot be empty"));
        }
        else if (newUser.Username.Length > 16)
        {
            return new Result<User>(new Exception("Username cannot be longer than 16 characters"));
        }
        else if (await userRepository.GetUserByUsername(newUser.Username) != null)
        {
            return new Result<User>(new Exception("Username already taken. Choose another username"));
        }
        else if (!Roles.Check(newUser.Role))
        {
            return new Result<User>(new Exception("Role is not valid"));
        }
         else if (newUser.Password.Length == 0) 
        {
            return new Result<User>(new Exception("Password cannot be empty!"));
        }
         else if (newUser.Password.Length < 16)
        {
            return new Result<User>(new Exception("Password cannot have less than 16 characters"));
        }
        else if (await userRepository.GetUserByUsername(newUser.Username) != null)
        {
            return new Result<User>(new Exception("Username already taken. Choose another username"));
        }

        
        newUser.Salt = Path.GetRandomFileName();
        newUser.Password = Encode(newUser.Password + newUser.Salt);
        User? createduser = await userRepository.Create(newUser);

        var result = (createduser == null) ?
            new Result<User>(new Exception("User Creation Failed")) :
            new Result<User>(createduser);
        return await Task.FromResult(result);

    }
    public async Task<Result<User>> Read(int id)
    {
        User? user = await userRepository.Read(id);

        var result = (user == null) ?
            new Result<User>(new Exception("User Not Read")) :
            new Result<User>(user);

        return await Task.FromResult(result);
    }
    public async Task<Result<User>> Update(int id, User newUser)
    {        
       if (string.IsNullOrWhiteSpace(newUser.Role))
        {
            newUser.Role = Roles.USER;
        }

        if (string.IsNullOrWhiteSpace(newUser.Username))
        {
            return new Result<User>(new Exception("Username cannot be empty"));
        }
        else if (newUser.Username.Length > 16)
        {
            return new Result<User>(new Exception("Username cannot be longer than 16 characters"));
        }
        if (string.IsNullOrWhiteSpace(newUser.Password))
        {
            return new Result<User>(new Exception("Password cannot be empty"));
        }
        else if (newUser.Password.Length < 16) // HAVE TO CHANGE AFTER TESTING///////////////////////////////////////////////////////////////
        {
            return new Result<User>(new Exception("Password cannot have less than 16 characters"));
        }
        else if (await userRepository.GetUserByUsername(newUser.Username) != null)
        {
            return new Result<User>(new Exception("Username already taken. Choose another username"));
        }
        else if (!Roles.Check(newUser.Role))
        {
            return new Result<User>(new Exception("Role is not valid"));
        }
       newUser.Salt = Path.GetRandomFileName();
       newUser.Password = Encode(newUser.Password + newUser.Salt);

        User? user = await userRepository.Update(id, newUser); 

        var result = (user == null) ?
            new Result<User>(new Exception("User Could not be Updated")) :
            new Result<User>(user);
        return await Task.FromResult(result);
    }
    public async Task<Result<User>> Delete(int id)
    {
        User? user = await userRepository.Delete(id);

        var result = (user == null) ?
            new Result<User>(new Exception("User Could Not Be Deleted")) :
            new Result<User>(user);
        return await Task.FromResult(result);
    }

    public async Task<Result<string>> GetToken(string username, string password)
    {
        User? user = await userRepository.GetUserByUsername(username);

        if (user != null && string.Equals(user.Password, Encode(password + user.Salt)))
        {
            return new Result<string>(Encode($@"username={user.Username}&role={user.Role}&expires={DateTime.Now.AddMinutes(60)}"));
        }
        else
        {
            return await Task.FromResult(new Result<string>(new Exception("Invalid username and password")));
        }       
    }

    public static string Encode(string plaintext)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(plaintext));
    }
    
    public static string Decode(string cyphertext)
    {
       return Encoding.UTF8.GetString(Convert.FromBase64String(cyphertext));
    }

    public async Task<Result<NameValueCollection>> ValidateToken(string token)
    {
        if (!string.IsNullOrWhiteSpace(token))
        {
            NameValueCollection? claims = HttpUtility.ParseQueryString(Decode(token));

            return new Result<NameValueCollection>(claims);
        }
        else
        {
            var result = new Result<NameValueCollection>(new Exception("Invalid token"));

            return await Task.FromResult(result);
        }
    }
}