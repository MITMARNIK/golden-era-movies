using GoldenEraMovies.Models;
using GoldenEraMovies.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoldenEraMovies.Services
{
    public class GenreService : IGenreService
    {
        private readonly IRepository<Genre> _repo;
        public GenreService(IRepository<Genre> repo) { _repo = repo; }
        public async Task<IEnumerable<Genre>> GetAllGenresAsync() => await _repo.GetAllAsync();
        public async Task<Genre> GetGenreByIdAsync(int id) => await _repo.GetByIdAsync(id);
        public async Task AddGenreAsync(Genre genre) => await _repo.AddAsync(genre);
        public async Task UpdateGenreAsync(Genre genre) => await _repo.UpdateAsync(genre);
        public async Task DeleteGenreAsync(int id) => await _repo.DeleteAsync(id);
    }
}
