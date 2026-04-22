using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoldenEraMovies.Models;
using GoldenEraMovies.Services;

namespace GoldenEraMovies.Controllers
{
    public class ActorsController : Controller
    {
        private readonly IActorService _actorService;
        public ActorsController(IActorService actorService) { _actorService = actorService; }

        // Index: afisare lista actori
        public async Task<IActionResult> Index() => View(await _actorService.GetAllActorsAsync());
        
        // Details: date complete actor
        public async Task<IActionResult> Details(int? id) { if (id == null) return NotFound(); var actor = await _actorService.GetActorByIdAsync(id.Value); return actor == null ? NotFound() : View(actor); }
        
        public IActionResult Create() => View();
        
        // Create: adaugare actor nou in DB
        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ActorId,FullName,ImagePath,OscarAwards,Bio")] Actor actor) { ModelState.Remove("MovieActors"); if (ModelState.IsValid) { await _actorService.AddActorAsync(actor); return RedirectToAction(nameof(Index)); } return View(actor); }
        
        public async Task<IActionResult> Edit(int? id) { if (id == null) return NotFound(); var actor = await _actorService.GetActorByIdAsync(id.Value); return actor == null ? NotFound() : View(actor); }
        
        // Edit: update date actor
        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ActorId,FullName,ImagePath,OscarAwards,Bio")] Actor actor) { if (id != actor.ActorId) return NotFound(); ModelState.Remove("MovieActors"); if (ModelState.IsValid) { await _actorService.UpdateActorAsync(actor); return RedirectToAction(nameof(Index)); } return View(actor); }
        
        public async Task<IActionResult> Delete(int? id) { if (id == null) return NotFound(); var actor = await _actorService.GetActorByIdAsync(id.Value); return actor == null ? NotFound() : View(actor); }
        
        // Delete: stergere inregistrare
        [HttpPost, ActionName("Delete")][ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) { await _actorService.DeleteActorAsync(id); return RedirectToAction(nameof(Index)); }
    }
}
