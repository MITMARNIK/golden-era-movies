using Microsoft.AspNetCore.Mvc;

namespace GoldenEraMovies.Controllers
{
    // Acest Controller gestioneaza pagina cu Actorii din Golden Era
    public class ActorsController : Controller
    {
        // GET: Actors/Index - Afiseaza lista de actori legendari
        public IActionResult Index()
        {
            return View(); // Trimite la vizualizarea Index.cshtml (din folderul Actors)
        }
    }
}
