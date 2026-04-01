using GoldenEraMovies.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GoldenEraMovies.Controllers
{
    // Acest Controller gestioneaza paginile generale ale site-ului (Home si Profil)
    public class HomeController : Controller
    {
        // _context este interfata noastra cu baza de date
        private readonly ApplicationDbContext _context;

        // Constructorul: Primim contextul bazei de date prin Dependency Injection
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Home/Index - Pagina principala a site-ului
        public async Task<IActionResult> Index()
        {
            // Luam doar primele 3 filme din baza de date, sortate dupa rating (Top Rated)
            var topMovies = await _context.Movies
                .OrderByDescending(m => m.AverageRating)
                .Take(3)
                .ToListAsync();
            
            return View(topMovies); // Trimitem cele 3 filme catre pagina de start (Home)
        }

        // GET: Home/Profile - Afiseaza pagina de profil a utilizatorului
        public IActionResult Profile()
        {
            return View(); // Trimite la vizualizarea Profile.cshtml
        }

        // Metoda standard pentru gestionarea erorilor neasteptate in aplicatie
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
