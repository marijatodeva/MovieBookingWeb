using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieWeb.Models;
using MovieWeb.Services;
using Newtonsoft.Json;

public class MovieController : Controller
{
    private readonly IMovieService _movieService;

    public MovieController(IMovieService movieService)
    {
        _movieService = movieService;
    }


    [HttpGet]
    public async Task<IActionResult> Index(string genre)
    {
        var movies = await _movieService.GetAllMovies();

        if (!string.IsNullOrEmpty(genre))
        {
            movies = movies.Where(m => m.Genre == genre).ToList();
        }

        var movieIndexVMs = new List<MovieIndexViewModel>();
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://localhost:7054/");

        foreach (var movie in movies)
        {
            var response = await client.GetAsync($"api/MovieShowing?movieId={movie.Id}");
            var showings = new List<MovieShowing>();

            if (response.IsSuccessStatusCode)
            {
                var showingsJson = await response.Content.ReadAsStringAsync();
                showings = JsonConvert.DeserializeObject<List<MovieShowing>>(showingsJson);
            }

            movieIndexVMs.Add(new MovieIndexViewModel
            {
                Movie = movie,
                Showings = showings
            });
        }

        ViewBag.CurrentGenre = genre;
        return View(movieIndexVMs);
    }




    public async Task<IActionResult> Details(int id)
    {
        var movie = await _movieService.GetMovieById(id);
        if (movie == null) return NotFound();
        return View(movie);
    }

    public async Task<IActionResult> Search(string query)
    {
        var movies = await _movieService.SearchMovies(query);
        if (movies.Count == 1)
            return RedirectToAction("Details", new { id = movies[0].Id });

        return View("Index", movies);
    }
    [HttpGet]
    public async Task<IActionResult> Buy(int id, int showingId)
    {
        var movie = await _movieService.GetMovieById(id);
        if (movie == null)
            return NotFound();

        return RedirectToAction("Ticket", "Ticket", new { movieId = id, showingId = showingId });
    }


    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Buy(TicketPageViewModel model)
    {
        if (model == null || model.SelectedShowingId == 0)
            return BadRequest("Showing not selected");

        var movie = await _movieService.GetMovieById(model.Movie.Id);
        if (movie == null)
            return NotFound();

        return RedirectToAction("Ticket", "Ticket", new
        {
            movieId = movie.Id,
            showingId = model.SelectedShowingId
        });
    }



    [Authorize]
    [Authorize(Roles = "Admin")]

    public async Task<IActionResult> Edit(int id)
    {
        var movie = await _movieService.GetMovieById(id);
        if (movie == null) return NotFound();
        return View(movie);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, Movie movie)
    {
        if (id != movie.Id) return BadRequest();

        if (ModelState.IsValid)
        {
            var updated = await _movieService.UpdateMovie(movie);
            if (updated) return RedirectToAction(nameof(Index));
            ModelState.AddModelError("", "Error updating movie");
        }

        return View(movie);
    }
    [Authorize]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddShowing(int id)
    {
        var movie = await _movieService.GetMovieById(id);
        if (movie == null) return NotFound();
        return View(movie);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddShowing(CreateMovieShowing showing)
    {
        if (!ModelState.IsValid)
        {
            var movie = await _movieService.GetMovieById(showing.MovieId);
            return View(movie);
        }

        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://localhost:7054/");

        var formData = new MultipartFormDataContent
    {
        { new StringContent(showing.MovieId.ToString()), "MovieId" },
        { new StringContent(showing.ShowingDate.ToString("yyyy-MM-ddTHH:mm:ssZ")), "ShowingDate" },
        { new StringContent(showing.ShowingTime), "ShowingTime" },
        { new StringContent(showing.HallId.ToString()), "HallId" }
    };

        var response = await client.PostAsync("api/MovieShowing", formData);
        var responseContent = await response.Content.ReadAsStringAsync();


        if (response.IsSuccessStatusCode)
            return RedirectToAction("Index");

        var movieFallback = await _movieService.GetMovieById(showing.MovieId);
        ModelState.AddModelError("", "Error adding showing");
        return View(movieFallback);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult AddMovie()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddMovie(Movie movie)
    {
        if (!ModelState.IsValid)
            return View(movie);

        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://localhost:7054/");

        var response = await client.PostAsJsonAsync("api/Movie", movie);

        if (response.IsSuccessStatusCode)
            return RedirectToAction(nameof(Index));

        ModelState.AddModelError("", "Error adding movie");
        return View(movie);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveShowing(int movieId)
    {
        var movie = await _movieService.GetMovieById(movieId);
        if (movie == null) return NotFound();

        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://localhost:7054/");
        var response = await client.GetAsync($"api/MovieShowing/all?movieId={movie.Id}");

        var showings = new List<MovieShowing>();
        if (response.IsSuccessStatusCode)
        {
            var showingsJson = await response.Content.ReadAsStringAsync();
            showings = JsonConvert.DeserializeObject<List<MovieShowing>>(showingsJson);
        }

        var vm = new MovieIndexViewModel
        {
            Movie = movie,
            Showings = showings
        };

        return View(vm);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveShowing(int showingId, int movieId)
    {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://localhost:7054/");

        var response = await client.DeleteAsync($"api/MovieShowing/{showingId}");

        if (response.IsSuccessStatusCode)
            return RedirectToAction("Index"); 

        TempData["Error"] = "Failed to remove showing";
        return RedirectToAction("Index");
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ConfirmRemoveShowing(int showingId, int movieId)
    {
        
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://localhost:7054/");

        HttpResponseMessage response = null;
        string responseContent = "";

        try
        {
            response = await client.DeleteAsync($"api/MovieShowing/{showingId}");
            responseContent = await response.Content.ReadAsStringAsync();
        }


        catch (Exception ex)
        {
            TempData["Error"] = $"Exception during deletion: {ex.Message}";
            return RedirectToAction("RemoveShowing", new { movieId });
        }

        if (response.IsSuccessStatusCode)

            return RedirectToAction("RemoveShowing", new { movieId });

        TempData["Error"] = $"Failed to remove showing. Status: {response.StatusCode}, Content: {responseContent}";

        return RedirectToAction("RemoveShowing", new { movieId });
    }


}

