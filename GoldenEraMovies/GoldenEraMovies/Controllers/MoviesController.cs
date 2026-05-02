using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoldenEraMovies.Services;
using GoldenEraMovies.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GoldenEraMovies.Controllers
{
    public class MoviesController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly ApplicationDbContext _context;

        public MoviesController(IMovieService movieService, ApplicationDbContext context) 
        { 
            _movieService = movieService; 
            _context = context;
        }

        // Listare generala filme
        public async Task<IActionResult> Index() => View(await _movieService.GetAllMoviesAsync());

        // Detalii film si incarcare resurse
        public async Task<IActionResult> Details(int? id) { if (id == null) return NotFound(); var movie = await _movieService.GetMovieByIdAsync(id.Value); return movie == null ? NotFound() : View(movie); }

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
        public async Task<IActionResult> AddToWatchlist(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index", "Account");
            }

            // Verificam daca filmul e deja in watchlist
            var exists = await _context.Watchlists
                .AnyAsync(w => w.UserId == userId && w.MovieId == id);

            if (!exists)
            {
                var watchlistItem = new Watchlist
                {
                    MovieId = id,
                    UserId = userId.Value,
                    AddedAt = DateTime.Now
                };

                _context.Watchlists.Add(watchlistItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Watchlist", "Home");
        }
    }
}
