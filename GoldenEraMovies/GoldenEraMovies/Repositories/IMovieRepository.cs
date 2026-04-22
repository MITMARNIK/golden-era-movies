using GoldenEraMovies.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoldenEraMovies.Repositories
{
    public interface IMovieRepository : IRepository<Movie>
    {
        Task<IEnumerable<Movie>> GetAllWithGenreAsync();
        Task<Movie> GetWithGenreAsync(int id);
        Task<IEnumerable<Movie>> GetTopMoviesByViewsAsync(int count);
        Task<IEnumerable<Movie>> GetTopRatedMoviesAsync(int count);
    }
}
