using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoldenEraMovies.Services;
using GoldenEraMovies.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace GoldenEraMovies.Controllers
{
    public class MoviesController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly IWatchlistService _watchlistService;
        private readonly UserManager<User> _userManager;
        private readonly IReviewService _reviewService;
        private readonly IUserService _userService;

        public MoviesController(
            IMovieService movieService, 
            IWatchlistService watchlistService, 
            UserManager<User> userManager,
            IReviewService reviewService,
            IUserService userService) 
        { 
            _movieService = movieService; 
            _watchlistService = watchlistService;
            _userManager = userManager;
            _reviewService = reviewService;
            _userService = userService;
        }

        // Listare generala filme
        public async Task<IActionResult> Index() => View(await _movieService.GetAllMoviesAsync());

        // Detalii film si incarcare resurse
        public async Task<IActionResult> Details(int? id) 
        { 
            if (id == null) return NotFound(); 
            var movie = await _movieService.GetMovieByIdAsync(id.Value); 
            if (movie == null) return NotFound(); 

            // Load reviews and author avatars
            var allReviews = await _reviewService.GetAllReviewsAsync();
            var movieReviews = allReviews.Where(r => r.MovieId == id.Value).OrderByDescending(r => r.CreatedAt).ToList();
            
            var userAvatars = new System.Collections.Generic.Dictionary<string, string>();
            foreach (var rev in movieReviews)
            {
                if (!string.IsNullOrEmpty(rev.UserId) && !userAvatars.ContainsKey(rev.UserId))
                {
                    var u = await _userService.GetUserByUsernameAsync(rev.UserId);
                    if (u != null)
                    {
                        userAvatars[rev.UserId] = u.ProfilePictureUrl;
                    }
                }
            }

            ViewBag.Reviews = movieReviews;
            ViewBag.UserAvatars = userAvatars;

            return View(movie); 
        }

        // Update rating via AJAX
        [HttpPost]
        public async Task<IActionResult> Rate(int id, int rating)
        {
            var result = await _movieService.RateMovieAsync(id, rating);
            if (!result.Success) return Json(new { success = false });
            return Json(new { success = true, newRating = result.NewRating });
        }

        // Top 10 filme dupa vizualizari
        public async Task<IActionResult> Top() => View(await _movieService.GetTopMoviesByViewsAsync(10));

        // Adaugare in Watchlist
        [Authorize]
        public async Task<IActionResult> AddToWatchlist(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Account");
            }

            await _watchlistService.AddToWatchlistAsync(user.Id, id);

            return RedirectToAction("Watchlist", "Home");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MovieId,Title,Description,ReleaseYear,Duration,PosterPath")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                await _movieService.AddMovieAsync(movie);
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _movieService.DeleteMovieAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
