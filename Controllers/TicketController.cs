using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieWeb.Models;
using MovieWeb.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MovieWeb.Controllers
{
    [Authorize]

    public class TicketController : Controller
    {
        private readonly string _apiBaseUrl = "https://localhost:7054/api";

        private readonly IMovieService _movieService;

        public TicketController(IMovieService movieService)
        {
            _movieService = movieService;
        }


        public async Task<IActionResult> Index()
        {
            List<TicketViewModel> tickets = new List<TicketViewModel>();

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{_apiBaseUrl}/Ticket");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    tickets = JsonConvert.DeserializeObject<List<TicketViewModel>>(json);
                }
            }
            return View(tickets);
        }

        [Authorize] 
        public async Task<IActionResult> MyTickets()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return RedirectToAction("Login", "AppUser");

            int userId = int.Parse(userIdClaim.Value);

            var tickets = await _movieService.GetTicketsByUserAsync(userId);

            return View(tickets); 
        }

     


        public async Task<IActionResult> Ticket(int movieId, DateTime? selectedDate, string selectedTime, int? selectedShowingId)
        {
            var movie = await _movieService.GetMovieById(movieId);

            var showings = selectedDate.HasValue
               ? await _movieService.GetShowingsForMovie(movieId, selectedDate)
               : new List<MovieShowing>();


            List<SeatViewModel> seatViewModels = new List<SeatViewModel>();
            int hallId = 0;

            if (selectedDate.HasValue && selectedShowingId.HasValue)
            {
                var showing = showings.FirstOrDefault(s => s.Id == selectedShowingId.Value);
                if (showing != null)
                {
                    hallId = showing.HallId;

                    var allSeats = await _movieService.GetSeatsForMovie(showing.Id);
                    var bookedSeats = await _movieService.GetBookedSeatsForShowing(showing.Id);

                    seatViewModels = allSeats.Select(s => new SeatViewModel
                    {
                        SeatNumber = s.SeatNumber,
                        IsBooked = bookedSeats.Contains(s.SeatNumber)
                    }).ToList();

                }
            }

            var vm = new TicketPageViewModel
            {
                Movie = movie,
                Showings = showings,
                Seats = seatViewModels,       
                SelectedDate = selectedDate,
                SelectedShowingId = selectedShowingId,
                HallId = hallId
            };

            return View(vm);
        }


        #region Helper Methods
        private async Task<Movie> GetMovieByIdAsync(int id)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/Movies/{id}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Movie>(json);
        }
        #endregion

        public IActionResult CreditCardForm(int movieId, DateTime selectedDate, string[] seats)
        {
            var model = new CreditCardPaymentModel
            {
                MovieId = movieId,
                WatchMovie = selectedDate,
            };
            return View(model);
        }
    }
}
