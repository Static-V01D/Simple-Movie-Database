namespace SMDB;

public class MockActorService : IActorService
{
    private  IActorRepository ActorRepository;

    public MockActorService(IActorRepository ActorRepository)
    {
        this.ActorRepository = ActorRepository;
    }
    
    public async Task<Result<PagedResult<Actor>>> ReadAll(int page, int Size)
    {
        var PagedResult = await ActorRepository.ReadAll(page, Size);
       
        var result = (PagedResult == null)?
            new Result<PagedResult<Actor>>(new Exception("No Results Found")):
            new Result<PagedResult<Actor>>(PagedResult);
        return await Task.FromResult(result);
    }
    public async Task<Result<Actor>> Create(Actor newActor)
     {
        if(string.IsNullOrWhiteSpace(newActor.FirstName))
        {
            return new Result<Actor>(new Exception("First name cannot be empty"));
        }
        else if(newActor.FirstName.Length > 16)
        {
            return new Result<Actor>(new Exception("First name cannot be longer than 16 characters"));
        }
        else if(string.IsNullOrWhiteSpace(newActor.LastName))
        {
            return new Result<Actor>(new Exception("Last name cannot be empty"));
        }
        else if(newActor.LastName.Length > 16)
        {
            return new Result<Actor>(new Exception("Last name cannot be longer than 16 characters"));
        }
        Actor? createdActor = await ActorRepository.Create(newActor);
       
        var result = (createdActor == null)?
            new Result<Actor>(new Exception("Actor Creation Failed")):
            new Result<Actor>(createdActor);
        return await Task.FromResult(result);
       
    }
    public async Task<Result<Actor>> Read(int id)
     {
        Actor? Actor = await ActorRepository.Read(id);
       
        var result = (Actor == null)?
            new Result<Actor>(new Exception("Actor Not Read")):
            new Result<Actor>(Actor);

        return await Task.FromResult(result);
    }
    public async Task<Result<Actor>> Update(int id, Actor newActor)
     {
       Actor? Actor = await ActorRepository.Update(id, newActor);
       
        var result = (Actor == null)?
            new Result<Actor>(new Exception("Actor Could not be Updated")):
            new Result<Actor>(Actor);
        return await Task.FromResult(result);
    }
    public async Task<Result<Actor>> Delete(int id)
     {
         Actor? Actor = await ActorRepository.Delete(id);
       
        var result = (Actor == null)?
            new Result<Actor>(new Exception("Actor Could Not Be Deleted")):
            new Result<Actor>(Actor);
        return await Task.FromResult(result);
    }
}