namespace SMDB;

public interface IActorMovieService
{
    public Task<Result<PagedResult<(ActorMovie,Movie)>>> ReadAllMoviesByActor(int actorId, int page, int size);
    public Task<Result<PagedResult<(ActorMovie,Actor)>>> ReadAllActorsByMovie(int movieId, int page, int size);
    public Task<Result<List<Actor>>> ReadAllActors() ; 
    public Task<Result<List<Movie>>> ReadAllMovies() ;   
    public Task<Result<ActorMovie>> create(int actorId, int movieId, string roleName);
    public Task<Result<ActorMovie>> Delete(int id);

}