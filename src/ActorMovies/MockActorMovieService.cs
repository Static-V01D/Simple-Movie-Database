namespace SMDB;

public class MockActorMovieService : IActorMovieService
{
    private IActorMovieRepository actorMovieRepository;

    public MockActorMovieService(IActorMovieRepository actorMovieRepository)
    {
        this.actorMovieRepository = actorMovieRepository;
    }
    public async Task<Result<PagedResult<(ActorMovie,Movie)>>> ReadAllMoviesByActor(int actorId, int page, int size)
    {

        var PagedResult = await actorMovieRepository.ReadAllMoviesByActor(actorId, page, size);

        var result = (PagedResult == null) ?
        new Result<PagedResult<(ActorMovie,Movie)>>(new Exception("No movies by actor Found")) :
        new Result<PagedResult<(ActorMovie,Movie)>>(PagedResult);

        return await Task.FromResult(result);
    }
    public async Task<Result<PagedResult<(ActorMovie,Actor)>>> ReadAllActorsByMovie(int movieId, int page, int size)
    {
        var PagedResult = await actorMovieRepository.ReadAllActorsByMovie(movieId, page, size);

        var result = (PagedResult == null) ?
        new Result<PagedResult<(ActorMovie,Actor)>>(new Exception("No actor by movie Found")) :
        new Result<PagedResult<(ActorMovie,Actor)>>(PagedResult);

        return await Task.FromResult(result);
    }
    public async Task<Result<List<Actor>>> ReadAllActors()
    {
        var PagedResult = await actorMovieRepository.ReadAllActors();

        var result = (PagedResult == null) ?
        new Result<List<Actor>>(new Exception("No actor Results Found")) :
        new Result<List<Actor>>(PagedResult);

        return await Task.FromResult(result);
    }
    public async Task<Result<List<Movie>>> ReadAllMovies()
    {
        var PagedResult = await actorMovieRepository.ReadAllMovies();

        var result = (PagedResult == null) ?
        new Result<List<Movie>>(new Exception("No movies Results Found")) :
        new Result<List<Movie>>(PagedResult);

        return await Task.FromResult(result);
    }
    public async Task<Result<ActorMovie>> create(int actorId, int movieId, string roleName)
    {
       ActorMovie? actorMovie = await actorMovieRepository.create(actorId,movieId,roleName);

        var result = (actorMovie == null) ?
        new Result<ActorMovie>(new Exception("ActorMovie could not be created")) :
        new Result<ActorMovie>(actorMovie);

        return await Task.FromResult(result);
    }
    public async Task<Result<ActorMovie>> Delete(int id)
    {
        ActorMovie? actorMovie = await actorMovieRepository.Delete(id);

        var result = (actorMovie == null) ?
        new Result<ActorMovie>(new Exception("ActorMovie could not be Deleted")) :
        new Result<ActorMovie>(actorMovie);

        return await Task.FromResult(result);
    }
}