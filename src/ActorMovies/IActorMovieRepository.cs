Namespace SMDB;

public interface IActorMovieRepository
{
    public Task<PagedResult<Movie>> ReadAllMoviesByActor(int actorId, int page, int size);
    public Task<PagedResult<Movie>> ReadAllActorsByMovie(int movieId, int page, int size);
    public Task<List<Actor>> ReadAllActors() ; 
    public Task<List<Movie>> ReadAllMovies() ;   
    public Task<ActorMovie> create(int actorId, int movieId, string roleName);
    public Task<ActorMovie> Delete(int id);

}