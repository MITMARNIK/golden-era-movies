using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GoldenEraMovies.Models;

namespace GoldenEraMovies.Controllers
{
    // Acest Controller gestioneaza tot ce tine de Filme (Afisare, Detalii, Top, Rating)
    public class MoviesController : Controller
    {
        // _context este "telecomanda" bazei de date (ApplicationDbContext)
        private readonly ApplicationDbContext _context;

        // Constructorul: Aici primim contextul bazei de date de la sistemul ASP.NET
        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Movies - Afiseaza lista principala de filme
        public async Task<IActionResult> Index()
        {
            // Luam filmele din DB si includem si Genul lor (Join intre tabele)
            var applicationDbContext = _context.Movies.Include(m => m.Genre);
            return View(await applicationDbContext.ToListAsync()); // Trimitem lista la pagina Index.cshtml
        }

        // GET: Movies/Details/id - Afiseaza pagina unui singur film
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound(); // Daca ID-ul lipseste, dam eroare

            // Cautam filmul dupa ID si aducem si informatiile despre Gen
            var movie = await _context.Movies
                .Include(m => m.Genre)
                .FirstOrDefaultAsync(m => m.MovieId == id);
            
            if (movie == null) return NotFound(); // Daca filmul nu exista in DB

            return View(movie); // Trimitem datele filmului la pagina Details.cshtml
        }

        // POST: Movies/Rate - Metoda care salveaza rating-ul nou trimis prin AJAX (jQuery)
        [HttpPost]
        public async Task<IActionResult> Rate(int id, int rating)
        {
            // 1. Cautam filmul in baza de date dupa ID
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return Json(new { success = false, message = "Movie not found" });

            // 2. Calculam noua medie: (MediaExistenta + VotulNou) / 2
            movie.AverageRating = (movie.AverageRating + (decimal)rating) / 2;

            // 3. Salvam schimbarile inapoi in baza de date
            await _context.SaveChangesAsync();

            // 4. Trimitem un raspuns JSON inapoi la codul JavaScript (jQuery)
            return Json(new { success = true, newRating = movie.AverageRating });
        }

        // GET: Movies/Top - Afiseaza Top 10 filme dupa numarul de vizualizari
        public async Task<IActionResult> Top()
        {
            var topMovies = await _context.Movies
                .Include(m => m.Genre)
                .OrderByDescending(m => m.ViewsCount) // Sortam descrescator dupa ViewsCount
                .Take(10) // Luam doar primele 10 rezultate
                .ToListAsync();
            return View(topMovies); // Trimitem lista la pagina Top.cshtml
        }

        // Metoda auxiliara: verifica daca un film exista (folosita intern)
        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.MovieId == id);
        }
    }
}
