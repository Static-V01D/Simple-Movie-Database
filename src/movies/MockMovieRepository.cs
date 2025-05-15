using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SMDB;

namespace SMDB
{
    public class MockMovieRepository: IMovieRepository
    {
        private readonly List<Movie> movies = new();
        private int idCount = 1;

        
        private readonly string[] titles = new[]
        {
            "The Shawshank Redemption", "The Godfather", "The Dark Knight", "Pulp Fiction",
            "The Lord of the Rings: The Return of the King", "Forrest Gump", "Inception",
            "Fight Club", "The Matrix", "Goodfellas", "Star Wars: The Empire Strikes Back",
            "Interstellar", "The Green Mile", "Gladiator", "The Lion King", "Saving Private Ryan",
            "The Silence of the Lambs", "Se7en", "Avengers: Endgame", "The Prestige"
        };

        private readonly int[] years = new[]
        {
            1994, 1972, 2008, 1994, 2003, 1994, 2010, 1999, 1999, 1990,
            1980, 2014, 1999, 2000, 1994, 1998, 1991, 1995, 2019, 2006
        };

        private readonly string[] descriptions = new[]
        {
            "A wrongly convicted man finds hope through friendship and perseverance.",
            "The patriarch of a crime family transfers power to his reluctant son.",
            "Batman faces the Joker in a gritty crime thriller.",
            "Stories collide in a violent, stylish tale of Los Angeles crime.",
            "The final stand against evil in Middle-earth begins.",
            "An extraordinary life unfolds through simple acts of kindness.",
            "Dreams become heists in a layered psychological thriller.",
            "An office worker discovers a secret fight club.",
            "A hacker learns the truth about his reality.",
            "A young man climbs the ranks of organized crime.",
            "Rebels face off against Darth Vader in a galactic showdown.",
            "A team of astronauts searches for a new home for mankind.",
            "A man with a special gift lives on death row.",
            "A general becomes a gladiator to seek vengeance.",
            "A young lion returns to reclaim his destiny.",
            "A mission to save one soldier turns into a battle.",
            "An FBI trainee interviews a cannibal to stop a killer.",
            "Two detectives chase a serial killer using the seven deadly sins.",
            "The Avengers face their most powerful foe yet.",
            "Two rival magicians push the limits of illusion."
        };

        private readonly float[] ratings = new[]
        {
            9.3f, 9.2f, 9.0f, 8.9f, 8.9f, 8.8f, 8.8f, 8.8f, 8.7f, 8.7f,
            8.7f, 8.6f, 8.6f, 8.5f, 8.5f, 8.5f, 8.6f, 8.6f, 8.4f, 8.5f
        };

        public MockMovieRepository()
        {
            for (int i = 0; i < 100; i++)
            {
                int index = i % titles.Length;
                movies.Add(new Movie
                {
                    Id = idCount++,
                    Title = titles[index] + (i >= titles.Length ? $" ({i + 1})" : ""),
                    Year = years[index],
                    Description = descriptions[index],
                    Rating = ratings[index]
                });
            }
        }
      

    private void GenerateMovies()
    {
        var random = new Random();

        for (int i = 0; i < 100; i++)
        {
            string title = titles[random.Next(titles.Length)];
            int year = years[random.Next(years.Length)];
            string description = descriptions[random.Next(descriptions.Length)];
            float rating = (float)Math.Round(random.NextDouble() * 10, 1); // Ranking: 0.0 - 10.0

            movies.Add(new Movie(idCount++, title, year, description, (int)rating));
        }
    }

    public async Task<PagedResult<Movie>> ReadAll(int page, int size)
    {
        int skip = (page - 1) * size;
        var pagedMovies = movies.Skip(skip).Take(size).ToList();
        return await Task.FromResult(new PagedResult<Movie>(pagedMovies, movies.Count));
    }

    public async Task<Movie?> Create(Movie movie)
    {
        movie.Id = idCount++;
        movies.Add(movie);
        return await Task.FromResult(movie);
    }

    public async Task<Movie?> Read(int id)
    {
        var movie = movies.FirstOrDefault(a => a.Id == id);
        return await Task.FromResult(movie);
    }

    public async Task<Movie?> Update(int id, Movie updatedMovie)
    {
        var movie = movies.FirstOrDefault(a => a.Id == id);
        if (movie != null)
        {
            movie.Title = updatedMovie.Title;
            movie.Year = updatedMovie.Year;
            movie.Description = updatedMovie.Description;
            movie.Rating = updatedMovie.Rating;
        }
        return await Task.FromResult(movie);
    }

    public async Task<Movie?> Delete(int id)
    {
        var movie = movies.FirstOrDefault(a => a.Id == id);
        if (movie != null)
        {
            movies.Remove(movie);
        }
        return await Task.FromResult(movie);
    }
    }
}


    

