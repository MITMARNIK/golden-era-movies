using GoldenEraMovies.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoldenEraMovies.Repositories
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        public ReviewRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Review>> GetAllWithMoviesAsync() => 
            await _dbSet.Include(r => r.Movie).ToListAsync();

        public async Task<Review> GetWithMovieAsync(int id) => 
            await _dbSet.Include(r => r.Movie).FirstOrDefaultAsync(m => m.ReviewId == id);
    }
}
