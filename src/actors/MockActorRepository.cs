namespace SMDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class MockActorRepository : IActorRepository
{
    private readonly List<Actor> actors;
    private int idCount = 0;

    private readonly string[] firstNames = new[]
    {
        "Emma", "Liam", "Olivia", "Noah", "Ava", "Elijah", "Sophia", "James", "Isabella", "Lucas",
        "Mia", "Mason", "Amelia", "Logan", "Harper", "Ethan", "Evelyn", "Jacob", "Abigail", "Michael",
        "Emily", "Daniel", "Elizabeth", "Henry", "Sofia", "Jackson", "Scarlett", "Sebastian", "Victoria", "Aiden",
        "Grace", "Matthew", "Chloe", "Samuel", "Camila", "David", "Aria", "Joseph", "Penelope", "Carter",
        "Lily", "Owen", "Riley", "Wyatt", "Nora", "John", "Hazel", "Jack", "Zoey", "Luke",
        "Aurora", "Jayden", "Ellie", "Dylan", "Hannah", "Levi", "Lillian", "Gabriel", "Addison", "Julian",
        "Layla", "Isaac", "Brooklyn", "Anthony", "Audrey", "Grayson", "Bella", "Andrew", "Claire", "Christopher",
        "Lucy", "Joshua", "Aaliyah", "Nathan", "Savannah", "Thomas", "Stella", "Caleb", "Natalie", "Ryan",
        "Leah", "Christian", "Skylar", "Hunter", "Violet", "Eli", "Genesis", "Isaiah", "Naomi", "Charles",
        "Elena", "Aaron", "Caroline", "Lincoln", "Anna", "Jonathan", "Allison", "Connor", "Alice", "Landon"
    };

    private readonly string[] lastNames = new[]
    {
        "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez",
        "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin",
        "Lee", "Perez", "Thompson", "White", "Harris", "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson",
        "Walker", "Young", "Allen", "King", "Wright", "Scott", "Torres", "Nguyen", "Hill", "Flores",
        "Green", "Adams", "Nelson", "Baker", "Hall", "Rivera", "Campbell", "Mitchell", "Carter", "Roberts",
        "Gomez", "Phillips", "Evans", "Turner", "Diaz", "Parker", "Cruz", "Edwards", "Collins", "Reyes",
        "Stewart", "Morris", "Morales", "Murphy", "Cook", "Rogers", "Gutierrez", "Ortiz", "Morgan", "Cooper",
        "Peterson", "Bailey", "Reed", "Kelly", "Howard", "Ramos", "Kim", "Cox", "Ward", "Richardson",
        "Watson", "Brooks", "Chavez", "Wood", "James", "Bennett", "Gray", "Mendoza", "Ruiz", "Hughes",
        "Price", "Alvarez", "Castillo", "Sanders", "Patel", "Myers", "Long", "Ross", "Foster", "Jimenez"
    };

    private readonly string[] bioPrompts = new[]
    {
        "Known for emotionally powerful performances in award-winning films.",
        "Started in indie films and rose to international fame.",
        "Highly versatile actor admired for dramatic and comedic roles.",
        "Frequently praised for on-screen charisma and dedication to craft.",
        "Featured in multiple blockbuster franchises and acclaimed TV series.",
        "Renowned for method acting and character immersion.",
        "Recipient of multiple accolades for outstanding performances.",
        "Brought to fame by a breakout role in a coming-of-age drama.",
        "Beloved by fans for authentic portrayals and humility.",
        "Respected in the industry for talent and philanthropy work."
    };

    public MockActorRepository()
    {
        actors = new List<Actor>();
        GenerateActors();
    }

    private void GenerateActors()
    {
        var random = new Random();

        for (int i = 0; i < 100; i++)
        {
            string firstName = firstNames[random.Next(firstNames.Length)];
            string lastName = lastNames[random.Next(lastNames.Length)];
            string bio = bioPrompts[random.Next(bioPrompts.Length)];
            float rating = (float)Math.Round(random.NextDouble() * 10, 1); // Ranking: 0.0 - 10.0

            actors.Add(new Actor(idCount++, firstName, lastName, bio, (int)rating));
        }
    }

    public async Task<PagedResult<Actor>> ReadAll(int page, int size)
    {
        int skip = (page - 1) * size;
        var pagedActors = actors.Skip(skip).Take(size).ToList();
        return await Task.FromResult(new PagedResult<Actor>(pagedActors, actors.Count));
    }

    public async Task<Actor?> Create(Actor actor)
    {
        actor.Id = idCount++;
        actors.Add(actor);
        return await Task.FromResult(actor);
    }

    public async Task<Actor?> Read(int id)
    {
        var actor = actors.FirstOrDefault(a => a.Id == id);
        return await Task.FromResult(actor);
    }

    public async Task<Actor?> Update(int id, Actor updatedActor)
    {
        var actor = actors.FirstOrDefault(a => a.Id == id);
        if (actor != null)
        {
            actor.FirstName = updatedActor.FirstName;
            actor.LastName = updatedActor.LastName;
            actor.Bio = updatedActor.Bio;
            actor.Rating = updatedActor.Rating;
        }
        return await Task.FromResult(actor);
    }

    public async Task<Actor?> Delete(int id)
    {
        var actor = actors.FirstOrDefault(a => a.Id == id);
        if (actor != null)
        {
            actors.Remove(actor);
        }
        return await Task.FromResult(actor);
    }
}
