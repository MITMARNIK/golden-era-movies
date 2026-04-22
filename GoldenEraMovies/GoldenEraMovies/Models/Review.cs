using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoldenEraMovies.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        public string CommentText { get; set; }

        [Required]
        [Range(1, 5)]
        public int StarRating { get; set; }

        public DateTime CreatedAt { get; set; }

        [Required]
        public int MovieId { get; set; }

        public Movie? Movie { get; set; }

        public string? UserId { get; set; }
    }
}
