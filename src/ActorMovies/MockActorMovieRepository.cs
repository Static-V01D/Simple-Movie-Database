namespace SMDB;

public class MockActorMovieRepository : IActorMovieRepository

{
    
    public interface IActorMovieRepository
    {
        private IActorRepository actorRepository;
        private IMovieRepository movieRepository;
        private List<ActorMovie> actorMovies;
        private int idCount;

        public MockActorMovieRepository(IActorRepository actorRepository, IMovieRepository movieRepository)
        {
            this.actorRepository = actorRepository;
            thismovieRepository = movieRepository;
            actorMovies = [];
            idCount = 0;
        }
        

        public async<PagedResult<List<Movie>>> ReadAllMoviesByActor(int actorId, int page, int size)
        {
            List<int> movieIds = actorMovies.FindAll((am) => am.ActorId == actorId).ConvertAll((am) => am.MovieId);
            List <Movie> movies = [];
            movies.ForEach(async (mid) => movies.Add((await movieRepository.Read(mid))!));
            
            int totalCount = movies.Count;
            int start = Math.Clamp((page -1)* size, 0, totalCount);
            int lenght = Math.Clamp(size, 0, totalCount - start);

            List<Movie> values = movies.Slice(start , lenght);

            var pagedResult = new PagedResult<Movie>(values, totalCount);

            return await Task.FromResult(pagedResult);
        }
        public async<PagedResult<Movie>> ReadAllActorsByMovie(int movieId, int page, int size)
        {
             List<int> actorIds = actorMovies.FindAll((am) => am.MovieId == movieId).ConvertAll((am) => am.ActorId);
            List <Actor> actors = [];
            actorIds.ForEach(async (aid) => movies.Add((await actorRepository.Read(aid))!));
            
            int totalCount = actors.Count;
            int start = Math.Clamp((page -1)* size, 0, totalCount);
            int lenght = Math.Clamp(size, 0, totalCount - start);
            List<Actor> values = actors.Slice(start , lenght);
            var pagedResult = new PagedResult<Actor>(values, totalCount);

            return await Task.FromResult(pagedResult);
        }
        public async<List<Actor>> ReadAllActors()  
        {
            var pagedResult = await actorRepository.ReadAll(1, int.MaxValue);
            return await Task.FromResult(pagedResult.Values);
        }
        public async<List<Movie>> ReadAllMovies()    
        {
            var pagedResult = await movieRepository.ReadAll(1, int.MaxValue);
            return await Task.FromResult(pagedResult.Values);
        }
        public async<ActorMovie?> create(int actorId, int movieId, string roleName)
        {
            var actorMovie = new ActorMovie(idCount++, actorId, movieId, roleName);

            actorMovies.Add(actorMovie);

            return await Task.FromResult(actorMovie);
            
        }
        public async<ActorMovie?> Delete(int id)
        {
            ActorMovie? actorMovie = actorMovies.FirstOrDefault((am) => am.Id == id);
            if(actorMovie != null)
            {
                actorMovies.Remove(actorMovie);
            }

            return await Task.FromResult(actorMovie);
        }

    }
  
}