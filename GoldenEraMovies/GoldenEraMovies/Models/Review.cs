using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoldenEraMovies.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        public string CommentText { get; set; }

        public int StarRating { get; set; }

        public DateTime CreatedAt { get; set; }

        // Legatura cu filmul (Foreign Key)
        public int MovieId { get; set; }

        [ForeignKey("MovieId")]
        public Movie Movie { get; set; }

        // Legatura cu utilizatorul (Identity)
        public string UserId { get; set; }
    }
}