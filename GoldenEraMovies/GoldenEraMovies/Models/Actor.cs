using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GoldenEraMovies.Models
{
    public class Actor
    {
        [Key]
        public int ActorId { get; set; }

        [Required]
        public string FullName { get; set; }

        public string ImagePath { get; set; }

        // Folosit pentru topul de actori
        public int OscarAwards { get; set; }

        public string Bio { get; set; }

        // Legătura  cu filmele
        public ICollection<MovieActor> MovieActors { get; set; }
    }
}