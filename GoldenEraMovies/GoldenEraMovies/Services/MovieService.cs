using GoldenEraMovies.Models;
using GoldenEraMovies.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoldenEraMovies.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepo;
        public MovieService(IMovieRepository movieRepo) { _movieRepo = movieRepo; }

        public async Task<IEnumerable<Movie>> GetAllMoviesAsync() => await _movieRepo.GetAllWithGenreAsync();
        public async Task<Movie> GetMovieByIdAsync(int id) => await _movieRepo.GetWithGenreAsync(id);
        public async Task<IEnumerable<Movie>> GetTopMoviesByViewsAsync(int count) => await _movieRepo.GetTopMoviesByViewsAsync(count);
        public async Task<IEnumerable<Movie>> GetTopRatedMoviesAsync(int count) => await _movieRepo.GetTopRatedMoviesAsync(count);
        public async Task<(bool Success, decimal NewRating)> RateMovieAsync(int id, int rating)
        {
            var movie = await _movieRepo.GetByIdAsync(id);
            if (movie == null) return (false, 0);
            movie.AverageRating = (movie.AverageRating + (decimal)rating) / 2;
            await _movieRepo.UpdateAsync(movie);
            return (true, movie.AverageRating);
        }
    }
}
