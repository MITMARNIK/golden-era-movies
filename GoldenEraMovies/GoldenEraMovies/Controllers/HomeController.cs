using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;
using GoldenEraMovies.Models;
using GoldenEraMovies.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace GoldenEraMovies.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly ApplicationDbContext _context;
        private readonly IMovieSyncService _syncService;
        private readonly IServiceProvider _serviceProvider;

        public HomeController(IMovieService movieService, ApplicationDbContext context, IMovieSyncService syncService, IServiceProvider serviceProvider) 
        { 
            _movieService = movieService; 
            _context = context;
            _syncService = syncService;
            _serviceProvider = serviceProvider;
        }

        public async Task<IActionResult> SyncData()
        {
            try
            {
                int count = await _syncService.SyncGoldenEraDataAsync();
                SyncState.LastSyncTime = DateTime.Now;
                TempData["Message"] = $"Sincronizare reusita! Am adaugat {count} filme noi.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Eroare la sincronizare: {ex.Message}. Asigura-te ca ai pus un API Key valid in appsettings.json.";
            }
            return RedirectToAction("Index");
        }

        // Pagina de start: Top 3 filme dupa rating + Sincronizare automata la refresh
        public async Task<IActionResult> Index() 
        {
            // Daca a trecut mai mult de 1 minut de la ultima sincronizare, pornim una noua in fundal
            if ((DateTime.Now - SyncState.LastSyncTime).TotalMinutes > 1)
            {
                SyncState.LastSyncTime = DateTime.Now;
                
                // Cream un scope separat pentru a evita eroarea "A second operation was started on this context"
                _ = Task.Run(async () => 
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var scopedSyncService = scope.ServiceProvider.GetRequiredService<IMovieSyncService>();
                        try { await scopedSyncService.SyncGoldenEraDataAsync(); } catch { }
                    }
                });
            }

            ViewBag.LastSync = SyncState.LastSyncTime == DateTime.MinValue ? "Never" : SyncState.LastSyncTime.ToString("HH:mm:ss");
            return View(await _movieService.GetTopRatedMoviesAsync(3));
        }

        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index", "Account");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            
            // Preluam ultimele 3 filme adaugate in watchlist
            ViewBag.RecentWatchlist = await _context.Watchlists
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.AddedAt)
                .Take(3)
                .Include(w => w.Movie)
                .Select(w => w.Movie)
                .ToListAsync();

            ViewBag.LastSync = SyncState.LastSyncTime == DateTime.MinValue ? "Never" : SyncState.LastSyncTime.ToString("HH:mm:ss");
            return View(user);
        }

        public async Task<IActionResult> Watchlist()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Index", "Account");

            var watchlistMovies = await _context.Watchlists
                .Where(w => w.UserId == userId)
                .Include(w => w.Movie)
                .Select(w => w.Movie)
                .ToListAsync();

            return View(watchlistMovies);
        }

        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return RedirectToAction("Index");
            }

            var model = new SearchViewModel
            {
                Query = query,
                Movies = await _context.Movies
                    .Include(m => m.Genre)
                    .Where(m => m.Title.Contains(query) || (m.Genre != null && m.Genre.Name.Contains(query)))
                    .ToListAsync(),
                Actors = await _context.Actors
                    .Where(a => a.FullName.Contains(query))
                    .ToListAsync(),
                Genres = await _context.Genres
                    .Where(g => g.Name.Contains(query))
                    .ToListAsync()
            };

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
