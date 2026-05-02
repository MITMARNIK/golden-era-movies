using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoldenEraMovies.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GoldenEraMovies.Controllers
{
    // Acest Controller gestioneaza paginile de Autentificare (Login si Register)
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

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

        // POST: Account/Register - Proceseaza inregistrarea
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Username,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                // Verificare daca userul e unic
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == user.Username);

                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Acest nume de utilizator este deja luat. Te rugam sa alegi altul.");
                    return View(user);
                }

                // Salvare user nou
                _context.Add(user);
                await _context.SaveChangesAsync();
                
                // Redirect catre login dupa inregistrare reusita
                return RedirectToAction(nameof(Login));
            }
            return View(user);
        }
        // POST: Account/Login - Proceseaza autentificarea
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Username,Password")] User user)
        {
            // Cautam userul in baza de date
            var dbUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == user.Username && u.Password == user.Password);

            if (dbUser != null)
            {
                // Autentificare reusita - Setam Sesiunea
                HttpContext.Session.SetString("UserSession", dbUser.Username);
                HttpContext.Session.SetInt32("UserId", dbUser.UserId);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Nume de utilizator sau parola incorecta.");
            return View(user);
        }

        // GET: Account/ChangePassword
        public IActionResult ChangePassword()
        {
            if (HttpContext.Session.GetInt32("UserId") == null) return RedirectToAction("Login");
            return View();
        }

        // POST: Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.Password != oldPassword)
            {
                ModelState.AddModelError(string.Empty, "Parola veche este incorecta.");
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Parolele noi nu coincid.");
                return View();
            }

            user.Password = newPassword;
            await _context.SaveChangesAsync();
            
            return RedirectToAction("Profile", "Home");
        }

        // GET: Account/Logout - Delogare
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
