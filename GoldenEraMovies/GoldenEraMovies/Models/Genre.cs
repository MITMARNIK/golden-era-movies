using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GoldenEraMovies.Models
{
    public class Genre
    {
        [Key]
        public int GenreId { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Movie> Movies { get; set; }
    }
}