namespace SMDB;

public class MockUserService : IUserService
{
    private  IUserRepository userRepository;

    public MockUserService(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }
    
    public async Task<Result<PagedResult<User>>> ReadAll(int page, int Size)
    {
        var PagedResult = await userRepository.ReadAll(page, Size);
       
        var result = (PagedResult == null)?
            new Result<PagedResult<User>>(new Exception("No Results Found")):
            new Result<PagedResult<User>>(PagedResult);
        return await Task.FromResult(result);
    }
    public async Task<Result<User>> Create(User newUser)
     {
        if(string.IsNullOrWhiteSpace(newUser.Username))
        {
            return new Result<User>(new Exception("User name cannot be empty"));
        }
        else if(newUser.Username.Length > 16)
        {
            return new Result<User>(new Exception("Username cannot be longer than 16 characters"));
        }
        
        User? createduser = await userRepository.Create(newUser);
       
        var result = (createduser == null)?
            new Result<User>(new Exception("User Creation Failed")):
            new Result<User>(createduser);
        return await Task.FromResult(result);
       
    }
    public async Task<Result<User>> Read(int id)
     {
        User? user = await userRepository.Read(id);
       
        var result = (user == null)?
            new Result<User>(new Exception("User Not Read")):
            new Result<User>(user);

        return await Task.FromResult(result);
    }
    public async Task<Result<User>> Update(int id, User newUser)
     {
       User? user = await userRepository.Update(id, newUser);
       
        if(string.IsNullOrWhiteSpace(newUser.Username))
        {
            return new Result<User>(new Exception("User name cannot be empty"));
        }
        else if(newUser.Username.Length > 16)
        {
            return new Result<User>(new Exception("Username cannot be longer than 16 characters"));
        }
       
        var result = (user == null)?
            new Result<User>(new Exception("User Could not be Updated")):
            new Result<User>(user);
        return await Task.FromResult(result);
    }
    public async Task<Result<User>> Delete(int id)
     {
         User? user = await userRepository.Delete(id);
       
        var result = (user == null)?
            new Result<User>(new Exception("User Could Not Be Deleted")):
            new Result<User>(user);
        return await Task.FromResult(result);
    }
}