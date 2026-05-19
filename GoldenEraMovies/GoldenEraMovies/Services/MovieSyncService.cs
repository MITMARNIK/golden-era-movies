using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using GoldenEraMovies.Models;
using GoldenEraMovies.Repositories;
using System.Text.Json.Serialization;

namespace GoldenEraMovies.Services
{
    public class MovieSyncService : IMovieSyncService
    {
        private readonly HttpClient _httpClient;
        private readonly IMovieRepository _movieRepo;
        private readonly IRepository<Genre> _genreRepo;
        private readonly IRepository<Actor> _actorRepo;
        private readonly IRepository<MovieActor> _movieActorRepo;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public MovieSyncService(
            HttpClient httpClient, 
            IMovieRepository movieRepo,
            IRepository<Genre> genreRepo,
            IRepository<Actor> actorRepo,
            IRepository<MovieActor> movieActorRepo,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _movieRepo = movieRepo;
            _genreRepo = genreRepo;
            _actorRepo = actorRepo;
            _movieActorRepo = movieActorRepo;
            _apiKey = configuration["TMDB:ApiKey"];
            _baseUrl = configuration["TMDB:BaseUrl"];
        }

        public async Task<int> SyncGoldenEraDataAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(_apiKey) || _apiKey == "57fa725031ebb32f5559f6ab4ccc81d6" == false && _apiKey == "YOUR_API_KEY_HERE")
                {
                    throw new Exception("TMDB API Key is not configured correctly.");
                }

                var url = $"{_baseUrl}discover/movie?api_key={_apiKey}&primary_release_date.gte=1930-01-01&primary_release_date.lte=1960-12-31&sort_by=popularity.desc&language=en-US";
                var response = await _httpClient.GetFromJsonAsync<TMDBResponse<TMDBMovie>>(url);
                if (response == null || response.Results == null) return 0;

                int syncedCount = 0;

                foreach (var tmdbMovie in response.Results.Take(15)) // Marim putin numarul de filme
                {
                    var movie = await _movieRepo.FirstOrDefaultAsync(m => m.Title == tmdbMovie.Title);
                    bool isNew = false;

                    var detailsUrl = $"{_baseUrl}movie/{tmdbMovie.Id}?api_key={_apiKey}&language=en-US";
                    var details = await _httpClient.GetFromJsonAsync<TMDBMovieDetails>(detailsUrl);

                    var genreName = details?.Genres?.FirstOrDefault()?.Name ?? "Classic";
                    var genre = await _genreRepo.FirstOrDefaultAsync(g => g.Name == genreName);
                    if (genre == null)
                    {
                        genre = new Genre { Name = genreName };
                        await _genreRepo.AddAsync(genre);
                    }

                    if (movie == null)
                    {
                        isNew = true;
                        movie = new Movie { Title = tmdbMovie.Title, ViewsCount = 0, GenreId = genre.GenreId };
                        await _movieRepo.AddAsync(movie);
                    }

                    // Actualizam campurile
                    movie.Description = tmdbMovie.Overview?.Length > 1000 ? tmdbMovie.Overview.Substring(0, 997) + "..." : tmdbMovie.Overview;
                    movie.ReleaseYear = DateTime.TryParse(tmdbMovie.ReleaseDate, out var date) ? date.Year : 1940;
                    movie.Duration = details?.Runtime ?? 100;
                    movie.AverageRating = tmdbMovie.VoteAverage / 2.0m;
                    movie.PosterPath = string.IsNullOrEmpty(tmdbMovie.PosterPath) ? "" : $"https://image.tmdb.org/t/p/w500{tmdbMovie.PosterPath}";
                    movie.GenreId = genre.GenreId;
                    movie.ViewsCount = (int)(tmdbMovie.Popularity * 100); 

                    try {
                        await _movieRepo.UpdateAsync(movie);
                    } catch (Exception ex) {
                        throw new Exception($"Eroare la salvarea/actualizarea filmului '{movie.Title}': {ex.Message} {ex.InnerException?.Message}");
                    }

                    // 5. Sincronizam Actorii
                    var creditsUrl = $"{_baseUrl}movie/{tmdbMovie.Id}/credits?api_key={_apiKey}";
                    var credits = await _httpClient.GetFromJsonAsync<TMDBCredits>(creditsUrl);

                    if (credits?.Cast != null)
                    {
                        foreach (var castMember in credits.Cast.Take(5))
                        {
                            var actor = await _actorRepo.FirstOrDefaultAsync(a => a.FullName == castMember.Name);
                            
                            var actorDetailsUrl = $"{_baseUrl}person/{castMember.Id}?api_key={_apiKey}&language=en-US";
                            var actorDetails = await _httpClient.GetFromJsonAsync<TMDBActorDetails>(actorDetailsUrl);

                            if (actor == null)
                            {
                                actor = new Actor { FullName = castMember.Name };
                                await _actorRepo.AddAsync(actor);
                            }

                            actor.ImagePath = string.IsNullOrEmpty(castMember.ProfilePath) ? "" : $"https://image.tmdb.org/t/p/w500{castMember.ProfilePath}";
                            actor.Bio = actorDetails?.Biography?.Length > 1000 ? actorDetails.Biography.Substring(0, 997) + "..." : (actorDetails?.Biography ?? (actor.Bio ?? ""));
                            
                            try {
                                await _actorRepo.UpdateAsync(actor);
                            } catch (Exception ex) {
                                throw new Exception($"Eroare la salvarea/actualizarea actorului '{actor.FullName}': {ex.Message} {ex.InnerException?.Message}");
                            }

                            var linkExists = await _movieActorRepo.FirstOrDefaultAsync(ma => ma.MovieId == movie.MovieId && ma.ActorId == actor.ActorId) != null;
                            if (!linkExists)
                            {
                                await _movieActorRepo.AddAsync(new MovieActor
                                {
                                    MovieId = movie.MovieId,
                                    ActorId = actor.ActorId,
                                    RoleName = castMember.Character ?? "Cast Member"
                                });
                            }
                        }
                    }
                    if (isNew) syncedCount++;
                }
                return syncedCount;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Clase suport pentru deserializare TMDB
        public class TMDBResponse<T> { [JsonPropertyName("results")] public List<T> Results { get; set; } }
        public class TMDBMovie 
        { 
            [JsonPropertyName("id")] public int Id { get; set; } 
            [JsonPropertyName("title")] public string Title { get; set; } 
            [JsonPropertyName("overview")] public string Overview { get; set; } 
            [JsonPropertyName("release_date")] public string ReleaseDate { get; set; } 
            [JsonPropertyName("vote_average")] public decimal VoteAverage { get; set; } 
            [JsonPropertyName("poster_path")] public string PosterPath { get; set; } 
            [JsonPropertyName("popularity")] public decimal Popularity { get; set; }
        }
        public class TMDBMovieDetails 
        { 
            [JsonPropertyName("runtime")] public int Runtime { get; set; } 
            [JsonPropertyName("genres")] public List<TMDBGenre> Genres { get; set; } 
        }
        public class TMDBGenre { [JsonPropertyName("name")] public string Name { get; set; } }
        public class TMDBCredits { [JsonPropertyName("cast")] public List<TMDBCast> Cast { get; set; } }
        public class TMDBCast 
        { 
            [JsonPropertyName("id")] public int Id { get; set; } 
            [JsonPropertyName("name")] public string Name { get; set; } 
            [JsonPropertyName("profile_path")] public string ProfilePath { get; set; } 
            [JsonPropertyName("character")] public string Character { get; set; } 
        }
        public class TMDBActorDetails 
        { 
            [JsonPropertyName("biography")] public string Biography { get; set; } 
        }
    }
}
