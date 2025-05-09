namespace SMDB;

public class MockUserRepository : IUserRepository
{
    private List<User>users;
    private int IdCount;
    public MockUserRepository()
    {
       users = [];
       IdCount = 1;

       var usernames = new string[]
       {
        "Papo","Pepe","Juan","Pedro","Pablo","Paco",
        "Luis","Jose","Javier","Jorge","Joaquin","Lilo",
       };

       Random r = new Random();

       foreach (var username in usernames)
       {
            var role = Roles.ROLES[r.Next(Roles.ROLES.Length)];
            var pass = Path.GetRandomFileName();
            var salt = Path.GetRandomFileName();
            User user = new User(IdCount++, username, pass, salt, role);

            users.Add(user);
       }
    }
    public async Task<PagedResult<User>> ReadAll(int page, int Size)
    {

        int totalCount = users.Count;
        int start = Math.Clamp((page - 1) * Size, 0, totalCount);
        int length = Math.Clamp(Size, 0, totalCount - start);
        List<User> values = users.Slice(start,length);
        var PagedResult = new PagedResult<User>(values, totalCount);

        return await Task.FromResult(PagedResult);
    }
    
    public async Task<User?> Create(User newUser)
    {
        newUser.Id = IdCount++;
        users.Add(newUser);       
        return await Task.FromResult(newUser);

    }
    public async Task<User?> Read(int id)
    {
        User? user = users.FirstOrDefault((u) => u.Id == id);

        return await Task.FromResult(user);
    }
    public async Task<User?> Update(int id, User newUser)
    {
        User? user = users.FirstOrDefault((u) => u.Id == id);
        if(user != null)
        {
            user.Username = newUser.Username;
            user.Password = newUser.Password;
            user.Salt = newUser.Salt;
            user.Role = newUser.Role;
        }

        return await Task.FromResult(user);
    }
    public async Task<User?> Delete(int id)
    {
        User? user = users.FirstOrDefault((u) => u.Id == id);
        if(user != null)
        {
            users.Remove(user);
           
        }
        return await Task.FromResult(user);
    }
}