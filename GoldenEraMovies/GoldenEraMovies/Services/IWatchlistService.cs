using GoldenEraMovies.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoldenEraMovies.Services
{
    public interface IWatchlistService
    {
        Task<IEnumerable<Movie>> GetRecentWatchlistMoviesAsync(int userId, int count);
        Task<IEnumerable<Movie>> GetUserWatchlistMoviesAsync(int userId);
        Task AddToWatchlistAsync(int userId, int movieId);
    }
}
