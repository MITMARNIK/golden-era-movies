using Microsoft.EntityFrameworkCore;
using GoldenEraMovies.Models;
using GoldenEraMovies.Services;
using GoldenEraMovies.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configurare Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

// Configurare Services
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IActorService, ActorService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddHttpClient<IMovieSyncService, MovieSyncService>();

// Configurare MVC
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configurare DB
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Sincronizare automata la pornirea aplicatiei (in fundal)
using (var scope = app.Services.CreateScope())
{
    var syncService = scope.ServiceProvider.GetRequiredService<IMovieSyncService>();
    // Pornim sincronizarea fara a astepta (fire and forget) pentru a nu bloca startul site-ului
    _ = Task.Run(async () => {
        try {
            await Task.Delay(5000); 
            await syncService.SyncGoldenEraDataAsync();
            SyncState.LastSyncTime = DateTime.Now;
        } catch { }
    });
}

app.Run();
