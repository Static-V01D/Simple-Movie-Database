namespace SMDB;

public class MockActorMovieRepository : IActorMovieRepository

{
    
    public interface IActorMovieRepository
    {
        private IActorRepository actorRepository;
        private IMovieRepository movieRepository;
        private List<ActorMovie> actorMovies

        public MockActorMovieRepository(IActorRepository actorRepository, IMovieRepository movieRepository)
        {
            this.actorRepository = actorRepository;
            thismovieRepository = movieRepository;
            actorMovies = [];

        }
        

        public async<PagedResult<List<Movie>>> ReadAllMoviesByActor(int actorId, int page, int size)
        {
            List<int> movieIds = actorMovies.FindAll((am) => am.ActorId == actorId).ConvertAll((am) => am.MovieId);
            List Movie movies = [];
            List<Movie> movies = movies.ForEach(async (mid) => movies.Add((await movieRepository.Read(mid))!));
            
            int totalCount = movies.Count;
            int start = Math.Clamp((page -1)* size)
        }
        public async<PagedResult<List<Movie>>> ReadAllActorsByMovie(int movieId)
        {

        }
        public async<List<Actor>> ReadAllActors()  
        {

        }
        public async<List<Movie>> ReadAllMovies()    
        {

        }
        public async<ActorMovie> create(int actorId, int movieId, string roleName)
        {

        }
        public async<ActorMovie> Delete(int id)
        {

        }

    }

}