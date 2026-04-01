using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoldenEraMovies.Models
{
    public class Watchlist
    {
        [Key]
        public int WatchlistId { get; set; }

        public DateTime AddedAt { get; set; }

        // Legatura cu filmul
        public int MovieId { get; set; }

        [ForeignKey("MovieId")]
        public Movie Movie { get; set; }

        // Legatura cu utilizatorul
        public string UserId { get; set; }
    }
}