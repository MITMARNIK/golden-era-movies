using GoldenEraMovies.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace GoldenEraMovies.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Watchlist> Watchlists { get; set; }
        public DbSet<MovieActor> MovieActors { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<MovieActor>().HasKey(ma => new { ma.MovieId, ma.ActorId });

            // Seeding legendary actors
            modelBuilder.Entity<Actor>().HasData(
                new Actor { ActorId = 1, FullName = "Katharine Hepburn", OscarAwards = 4, Bio = "The most awarded actress in history, known for her fierce independence and spirited personality.", ImagePath = "https://upload.wikimedia.org/wikipedia/commons/5/5b/Katharine_Hepburn_promo_photo.jpg" },
                new Actor { ActorId = 2, FullName = "Meryl Streep", OscarAwards = 3, Bio = "Often described as the best actress of her generation, with a record 21 Oscar nominations.", ImagePath = "https://upload.wikimedia.org/wikipedia/commons/4/44/Meryl_Streep_by_Jack_Mitchell.jpg" },
                new Actor { ActorId = 3, FullName = "Jack Nicholson", OscarAwards = 3, Bio = "Known for playing a wide range of starring or supporting roles, including satirical comedy and romance.", ImagePath = "https://upload.wikimedia.org/wikipedia/commons/3/3a/Jack_Nicholson_2001.jpg" },
                new Actor { ActorId = 4, FullName = "Marlon Brando", OscarAwards = 2, Bio = "Considered one of the most influential actors of the 20th century, famous for 'The Godfather'.", ImagePath = "https://upload.wikimedia.org/wikipedia/commons/5/53/Marlon_Brando_publicity_for_One-Eyed_Jacks.png" },
                new Actor { ActorId = 5, FullName = "Robert De Niro", OscarAwards = 2, Bio = "A versatile actor known for his intense method acting and collaborations with Martin Scorsese.", ImagePath = "https://upload.wikimedia.org/wikipedia/commons/5/58/Robert_De_Niro_Cannes_2016.jpg" }
            );
        }
    }
}