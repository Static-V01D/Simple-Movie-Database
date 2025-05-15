namespace SMDB;

public interface IActorRepository
{
    public Task<PagedResult<Actor>> ReadAll(int page, int Size);
    public Task<Actor?> Create(Actor actor);
    public Task<Actor?> Read(int id);
    public Task<Actor?> Update(int id, Actor newActor);
    public Task<Actor?> Delete(int id);


}