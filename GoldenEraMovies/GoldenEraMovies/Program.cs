using Microsoft.EntityFrameworkCore;
using GoldenEraMovies.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Adăugăm serviciile pentru Controller și View
builder.Services.AddControllersWithViews();

// 2. ÎNREGISTRAREA BAZEI DE DATE 
// Această linie citește conexiunea din appsettings.json și o dă spre folosire Contextului
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configurarea pipeline-ului HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection(); //Forteaza browserul sa folosească o conexiune sigura (HTTPS)
app.UseStaticFiles(); //Ii da voie serverului sa trimita fisierele din folderul wwwroot
app.UseRouting();       //Analizeaza adresa scrisa in browser (URL-ul) si decide unde trebuie sa mearga cererea
app.UseAuthorization(); //Verifica daca utilizatorul are voie sa vada o anumita pagina

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();