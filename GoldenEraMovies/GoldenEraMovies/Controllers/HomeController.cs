using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;
using GoldenEraMovies.Models;
using GoldenEraMovies.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace GoldenEraMovies.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly IActorService _actorService;
        private readonly IGenreService _genreService;
        private readonly IUserService _userService;
        private readonly IWatchlistService _watchlistService;
        private readonly IMovieSyncService _syncService;
        private readonly IServiceProvider _serviceProvider;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public HomeController(
            IMovieService movieService, 
            IActorService actorService,
            IGenreService genreService,
            IUserService userService,
            IWatchlistService watchlistService,
            IMovieSyncService syncService, 
            IServiceProvider serviceProvider,
            UserManager<User> userManager,
            SignInManager<User> signInManager) 
        { 
            _movieService = movieService; 
            _actorService = actorService;
            _genreService = genreService;
            _userService = userService;
            _watchlistService = watchlistService;
            _syncService = syncService;
            _serviceProvider = serviceProvider;
            _userManager = userManager;
            _signInManager = signInManager;
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
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Account");
            }
            
            var userId = user.Id;
            
            // Preluam ultimele 3 filme adaugate in watchlist
            ViewBag.RecentWatchlist = await _watchlistService.GetRecentWatchlistMoviesAsync(userId, 3);

            ViewBag.LastSync = SyncState.LastSyncTime == DateTime.MinValue ? "Never" : SyncState.LastSyncTime.ToString("HH:mm:ss");
            return View(user);
        }

        public async Task<IActionResult> Watchlist()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Index", "Account");

            var watchlistMovies = await _watchlistService.GetUserWatchlistMoviesAsync(user.Id);

            return View(watchlistMovies);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Index", "Account");

            var model = new EditProfileViewModel
            {
                UserName = user.UserName,
                ProfilePictureUrl = user.ProfilePictureUrl
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Index", "Account");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Update Username
            if (user.UserName != model.UserName)
            {
                var setUsernameResult = await _userManager.SetUserNameAsync(user, model.UserName);
                if (!setUsernameResult.Succeeded)
                {
                    foreach (var error in setUsernameResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }

            // Update Profile Picture
            user.ProfilePictureUrl = model.ProfilePictureUrl;
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            // Password Change Logic (if new password provided)
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                if (string.IsNullOrEmpty(model.CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "Current password is required to set a new password.");
                    return View(model);
                }

                var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    foreach (var error in changePasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }

            // Refresh user session so that the updated username/cookie is reflected
            await _signInManager.RefreshSignInAsync(user);

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("Profile");
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
                Movies = (await _movieService.SearchMoviesAsync(query)).ToList(),
                Actors = (await _actorService.SearchActorsAsync(query)).ToList(),
                Genres = (await _genreService.SearchGenresAsync(query)).ToList()
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
