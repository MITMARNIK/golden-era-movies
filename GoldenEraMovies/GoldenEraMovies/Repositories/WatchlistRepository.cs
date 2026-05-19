using GoldenEraMovies.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldenEraMovies.Repositories
{
    public class WatchlistRepository : Repository<Watchlist>, IWatchlistRepository
    {
        public WatchlistRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> ExistsAsync(int userId, int movieId)
        {
            return await _dbSet.AnyAsync(w => w.UserId == userId && w.MovieId == movieId);
        }

        public async Task<IEnumerable<Watchlist>> GetRecentWatchlistWithMoviesAsync(int userId, int count)
        {
            return await _dbSet
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.AddedAt)
                .Take(count)
                .Include(w => w.Movie)
                .ToListAsync();
        }

        public async Task<IEnumerable<Watchlist>> GetUserWatchlistWithMoviesAsync(int userId)
        {
            return await _dbSet
                .Where(w => w.UserId == userId)
                .Include(w => w.Movie)
                .ToListAsync();
        }
    }
}
