namespace SMDB;

public interface IMovieService
{
    public Task<Result<PagedResult<Movie>>> ReadAll(int page, int Size);
    public Task<Result<Movie>> Create(Movie Movie);
    public Task<Result<Movie>> Read(int id);
    public Task<Result<Movie>> Update(int id, Movie newMovie);
    public Task<Result<Movie>> Delete(int id);
}