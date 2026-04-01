using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoldenEraMovies.Models
{
    public class Movie
    {
        [Key] 
        public int MovieId { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public string PosterPath { get; set; }

        public decimal AverageRating { get; set; }

        public int ViewsCount { get; set; }

        public int ReleaseYear { get; set; }

        public int Duration { get; set; }

        public int GenreId { get; set; }

        [ForeignKey("GenreId")]
        public Genre Genre { get; set; }
    }
}
