namespace SMDB;

public class MockMovieService : IMovieService
{
    private  IMovieRepository MovieRepository;

    public MockMovieService(IMovieRepository MovieRepository)
    {
        this.MovieRepository = MovieRepository;
    }
    
    public async Task<Result<PagedResult<Movie>>> ReadAll(int page, int Size)
    {
        var PagedResult = await MovieRepository.ReadAll(page, Size);
       
        var result = (PagedResult == null)?
            new Result<PagedResult<Movie>>(new Exception("No Results Found")):
            new Result<PagedResult<Movie>>(PagedResult);
        return await Task.FromResult(result);
    }
    public async Task<Result<Movie>> Create(Movie newMovie)
     {
        if(string.IsNullOrWhiteSpace(newMovie.Title))
        {
            return new Result<Movie>(new Exception("Title cannot be empty"));
        }
        else if(newMovie.Title.Length > 256)
        {
            return new Result<Movie>(new Exception("Title cannot be longer than 256 characters"));
        }
        else if(newMovie.Year > DateTime.Now.Year)
        {
            return new Result<Movie>(new Exception("Year cannot be in the future"));
        }
        
        Movie? createdMovie = await MovieRepository.Create(newMovie);
       
        var result = (createdMovie == null)?
            new Result<Movie>(new Exception("Movie Creation Failed")):
            new Result<Movie>(createdMovie);
        return await Task.FromResult(result);
       
    }
    public async Task<Result<Movie>> Read(int id)
     {
        Movie? Movie = await MovieRepository.Read(id);
       
        var result = (Movie == null)?
            new Result<Movie>(new Exception("Movie Not Read")):
            new Result<Movie>(Movie);

        return await Task.FromResult(result);
    }
    public async Task<Result<Movie>> Update(int id, Movie newMovie)
     {

        if(string.IsNullOrWhiteSpace(newMovie.Title))
        {
            return new Result<Movie>(new Exception("Title cannot be empty"));
        }
        else if(newMovie.Title.Length > 256)
        {
            return new Result<Movie>(new Exception("Title cannot be longer than 256 characters"));
        }
        else if(newMovie.Year > DateTime.Now.Year)
        {
            return new Result<Movie>(new Exception("Year cannot be in the future"));
        }

       Movie? Movie = await MovieRepository.Update(id, newMovie);
       
        var result = (Movie == null)?
            new Result<Movie>(new Exception("Movie Could not be Updated")):
            new Result<Movie>(Movie);
        return await Task.FromResult(result);
    }
    public async Task<Result<Movie>> Delete(int id)
     {
         Movie? Movie = await MovieRepository.Delete(id);
       
        var result = (Movie == null)?
            new Result<Movie>(new Exception("Movie Could Not Be Deleted")):
            new Result<Movie>(Movie);
        return await Task.FromResult(result);
    }
}