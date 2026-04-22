using GoldenEraMovies.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldenEraMovies.Repositories
{
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
        public MovieRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Movie>> GetAllWithGenreAsync() => 
            await _dbSet.Include(m => m.Genre).ToListAsync();

        public async Task<Movie> GetWithGenreAsync(int id) => 
            await _dbSet.Include(m => m.Genre).FirstOrDefaultAsync(m => m.MovieId == id);

        public async Task<IEnumerable<Movie>> GetTopMoviesByViewsAsync(int count) => 
            await _dbSet.Include(m => m.Genre).OrderByDescending(m => m.ViewsCount).Take(count).ToListAsync();

        public async Task<IEnumerable<Movie>> GetTopRatedMoviesAsync(int count) => 
            await _dbSet.OrderByDescending(m => m.AverageRating).Take(count).ToListAsync();
    }
}
