using GoldenEraMovies.Models;
using GoldenEraMovies.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoldenEraMovies.Services
{
    public class ActorService : IActorService
    {
        private readonly IRepository<Actor> _repo;
        public ActorService(IRepository<Actor> repo) { _repo = repo; }
        public async Task<IEnumerable<Actor>> GetAllActorsAsync() => await _repo.GetAllAsync();
        public async Task<Actor> GetActorByIdAsync(int id) => await _repo.GetByIdAsync(id);
        public async Task AddActorAsync(Actor actor) => await _repo.AddAsync(actor);
        public async Task UpdateActorAsync(Actor actor) => await _repo.UpdateAsync(actor);
        public async Task DeleteActorAsync(int id) => await _repo.DeleteAsync(id);
    }
}
