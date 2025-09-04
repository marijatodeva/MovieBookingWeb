using Microsoft.AspNetCore.Mvc;
using MovieWeb.Models;
using MovieWeb.Services;
using System.Linq;
using System.Threading.Tasks;

namespace MovieWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMovieService _movieService;

        public HomeController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        public async Task<IActionResult> Index()
        {
            var allMovies = await _movieService.GetAllMovies();

            var top3Movies = allMovies.OrderByDescending(m => m.Price) 
                                      .Take(3)
                                      .ToList();

            return View(top3Movies); 
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
