using System.Threading.Tasks;

namespace GoldenEraMovies.Services
{
    public interface IMovieSyncService
    {
        Task<int> SyncGoldenEraDataAsync();
    }
}
