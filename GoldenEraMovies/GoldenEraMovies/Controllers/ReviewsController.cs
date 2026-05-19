using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GoldenEraMovies.Models;
using GoldenEraMovies.Services;

namespace GoldenEraMovies.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly IMovieService _movieService;

        public ReviewsController(IReviewService reviewService, IMovieService movieService) 
        { 
            _reviewService = reviewService; 
            _movieService = movieService;
        }

        // Listare recenzii
        public async Task<IActionResult> Index() => View(await _reviewService.GetAllReviewsAsync());
        
        // Detalii recenzie
        public async Task<IActionResult> Details(int? id) { if (id == null) return NotFound(); var review = await _reviewService.GetReviewByIdAsync(id.Value); return review == null ? NotFound() : View(review); }
        
        // Formular creare - Folosim movieService pentru dropdown
        public async Task<IActionResult> Create() 
        { 
            var movies = await _movieService.GetAllMoviesAsync();
            ViewData["MovieId"] = new SelectList(movies, "MovieId", "Title"); 
            return View(); 
        }
        
        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReviewId,CommentText,StarRating,MovieId")] Review review) 
        { 
            ModelState.Remove("Movie"); 
            ModelState.Remove("UserId");
            if (ModelState.IsValid) 
            { 
                review.UserId = User.Identity.IsAuthenticated ? User.Identity.Name : "Anonymous";
                review.CreatedAt = DateTime.Now; 
                await _reviewService.AddReviewAsync(review); 
                return RedirectToAction("Details", "Movies", new { id = review.MovieId }); 
            } 
            return RedirectToAction("Details", "Movies", new { id = review.MovieId }); 
        }
        
        public async Task<IActionResult> Edit(int? id) 
        { 
            if (id == null) return NotFound(); 
            var review = await _reviewService.GetReviewByIdAsync(id.Value); 
            if (review == null) return NotFound(); 
            var movies = await _movieService.GetAllMoviesAsync();
            ViewData["MovieId"] = new SelectList(movies, "MovieId", "Title", review.MovieId); 
            return View(review); 
        }
        
        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReviewId,CommentText,StarRating,CreatedAt,MovieId,UserId")] Review review) 
        { 
            if (id != review.ReviewId) return NotFound(); 
            ModelState.Remove("Movie"); 
            if (ModelState.IsValid) 
            { 
                await _reviewService.UpdateReviewAsync(review); 
                return RedirectToAction("Details", "Movies", new { id = review.MovieId }); 
            } 
            return RedirectToAction("Details", "Movies", new { id = review.MovieId }); 
        }
        
        [HttpPost, ActionName("Delete")][ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) 
        { 
            var review = await _reviewService.GetReviewByIdAsync(id);
            if (review != null)
            {
                await _reviewService.DeleteReviewAsync(id); 
                return RedirectToAction("Details", "Movies", new { id = review.MovieId }); 
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
