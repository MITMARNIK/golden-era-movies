using System.Collections.Generic;

namespace GoldenEraMovies.Models
{
    public class SearchViewModel
    {
        public string Query { get; set; }
        public List<Movie> Movies { get; set; } = new List<Movie>();
        public List<Actor> Actors { get; set; } = new List<Actor>();
        public List<Genre> Genres { get; set; } = new List<Genre>();
    }
}
