using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoldenEraMovies.Services;

namespace GoldenEraMovies.Controllers
{
    public class MoviesController : Controller
    {
        private readonly IMovieService _movieService;
        public MoviesController(IMovieService movieService) { _movieService = movieService; }

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
    }
}
