using GoldenEraMovies.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoldenEraMovies.Services
{
    public interface IMovieService
    {
        Task<IEnumerable<Movie>> GetAllMoviesAsync();
        Task<Movie> GetMovieByIdAsync(int id);
        Task<IEnumerable<Movie>> GetTopMoviesByViewsAsync(int count);
        Task<IEnumerable<Movie>> GetTopRatedMoviesAsync(int count);
        Task<(bool Success, decimal NewRating)> RateMovieAsync(int id, int rating);
    }
}
