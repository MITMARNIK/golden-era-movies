using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoldenEraMovies.Models;
using GoldenEraMovies.Services;

namespace GoldenEraMovies.Controllers
{
    public class GenresController : Controller
    {
        private readonly IGenreService _genreService;
        public GenresController(IGenreService genreService) { _genreService = genreService; }

        // Listare generala genuri
        public async Task<IActionResult> Index() => View(await _genreService.GetAllGenresAsync());
        
        // Detalii gen dupa ID
        public async Task<IActionResult> Details(int? id) { if (id == null) return NotFound(); var genre = await _genreService.GetGenreByIdAsync(id.Value); return genre == null ? NotFound() : View(genre); }
        
        public IActionResult Create() => View();
        
        // Salvare gen nou
        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GenreId,Name")] Genre genre) { ModelState.Remove("Movies"); if (ModelState.IsValid) { await _genreService.AddGenreAsync(genre); return RedirectToAction(nameof(Index)); } return View(genre); }
        
        public async Task<IActionResult> Edit(int? id) { if (id == null) return NotFound(); var genre = await _genreService.GetGenreByIdAsync(id.Value); return genre == null ? NotFound() : View(genre); }
        
        // Update gen existent
        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GenreId,Name")] Genre genre) { if (id != genre.GenreId) return NotFound(); ModelState.Remove("Movies"); if (ModelState.IsValid) { await _genreService.UpdateGenreAsync(genre); return RedirectToAction(nameof(Index)); } return View(genre); }
        
        public async Task<IActionResult> Delete(int? id) { if (id == null) return NotFound(); var genre = await _genreService.GetGenreByIdAsync(id.Value); return genre == null ? NotFound() : View(genre); }
        
        // Stergere confirmata gen
        [HttpPost, ActionName("Delete")][ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) { await _genreService.DeleteGenreAsync(id); return RedirectToAction(nameof(Index)); }
    }
}
