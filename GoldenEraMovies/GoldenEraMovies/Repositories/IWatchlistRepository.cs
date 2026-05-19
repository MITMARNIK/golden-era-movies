using GoldenEraMovies.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoldenEraMovies.Repositories
{
    public interface IWatchlistRepository : IRepository<Watchlist>
    {
        Task<IEnumerable<Watchlist>> GetRecentWatchlistWithMoviesAsync(int userId, int count);
        Task<IEnumerable<Watchlist>> GetUserWatchlistWithMoviesAsync(int userId);
        Task<bool> ExistsAsync(int userId, int movieId);
    }
}
