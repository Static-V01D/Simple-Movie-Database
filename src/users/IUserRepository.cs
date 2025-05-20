namespace SMDB;

public interface IUserRepository
{
    public Task<PagedResult<User>> ReadAll(int page, int Size);
    public Task<User?> Create(User user);
    public Task<User?> Read(int id);
    public Task<User?> Update(int id, User newUser);
    public Task<User?> Delete(int id);
    public Task<User?> GetUserByUsername(string username);


}