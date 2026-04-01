using Microsoft.AspNetCore.Mvc;

namespace GoldenEraMovies.Controllers
{
    // Acest Controller gestioneaza paginile de Autentificare (Login si Register)
    public class AccountController : Controller
    {
        // GET: Account/Index - Pagina de bun venit unde alegi intre Login si Register
        public IActionResult Index()
        {
            return View("LoginRegisterChose"); // Trimite la vizualizarea de selectie
        }

        // GET: Account/Login - Afiseaza formularul de Login
        public IActionResult Login()
        {
            return View(); // Trimite la vizualizarea Login.cshtml
        }

        // GET: Account/Register - Afiseaza formularul de Inregistrare
        public IActionResult Register()
        {
            return View(); // Trimite la vizualizarea Register.cshtml
        }
    }
}
