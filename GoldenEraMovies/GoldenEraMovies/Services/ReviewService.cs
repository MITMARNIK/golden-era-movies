using GoldenEraMovies.Models;
using GoldenEraMovies.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoldenEraMovies.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _repo;
        public ReviewService(IReviewRepository repo) { _repo = repo; }
        public async Task<IEnumerable<Review>> GetAllReviewsAsync() => await _repo.GetAllWithMoviesAsync();
        public async Task<Review> GetReviewByIdAsync(int id) => await _repo.GetWithMovieAsync(id);
        public async Task AddReviewAsync(Review review) => await _repo.AddAsync(review);
        public async Task UpdateReviewAsync(Review review) => await _repo.UpdateAsync(review);
        public async Task DeleteReviewAsync(int id) => await _repo.DeleteAsync(id);
    }
}
