using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;
using GoldenEraMovies.Models;
using GoldenEraMovies.Services;

namespace GoldenEraMovies.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMovieService _movieService;
        public HomeController(IMovieService movieService) { _movieService = movieService; }

        // Pagina de start: Top 3 filme dupa rating
        public async Task<IActionResult> Index() => View(await _movieService.GetTopRatedMoviesAsync(3));

        public IActionResult Profile() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
