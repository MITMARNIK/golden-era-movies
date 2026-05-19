using GoldenEraMovies.Models;
using GoldenEraMovies.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldenEraMovies.Services
{
    public class WatchlistService : IWatchlistService
    {
        private readonly IWatchlistRepository _watchlistRepository;

        public WatchlistService(IWatchlistRepository watchlistRepository)
        {
            _watchlistRepository = watchlistRepository;
        }

        public async Task AddToWatchlistAsync(int userId, int movieId)
        {
            var exists = await _watchlistRepository.ExistsAsync(userId, movieId);
            if (!exists)
            {
                var watchlistItem = new Watchlist
                {
                    MovieId = movieId,
                    UserId = userId,
                    AddedAt = DateTime.Now
                };
                await _watchlistRepository.AddAsync(watchlistItem);
            }
        }

        public async Task<IEnumerable<Movie>> GetRecentWatchlistMoviesAsync(int userId, int count)
        {
            var watchlists = await _watchlistRepository.GetRecentWatchlistWithMoviesAsync(userId, count);
            return watchlists.Select(w => w.Movie).ToList();
        }

        public async Task<IEnumerable<Movie>> GetUserWatchlistMoviesAsync(int userId)
        {
            var watchlists = await _watchlistRepository.GetUserWatchlistWithMoviesAsync(userId);
            return watchlists.Select(w => w.Movie).ToList();
        }
    }
}
