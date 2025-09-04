using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MovieWeb.Models;
using MovieWeb.Models.System;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MovieWeb.Services
{
    public class MovieService : IMovieService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;


        public MovieService(IOptions<DBSettings> settings)
        {
            _httpClient = new HttpClient();
            _apiBaseUrl = settings.Value.DbApi ?? throw new ArgumentNullException(nameof(settings.Value.DbApi));
        }

        public async Task<List<Movie>> GetAllMovies()
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Movies");
            if (!response.IsSuccessStatusCode) return new List<Movie>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Movie>>(json) ?? new List<Movie>();
        }

        public async Task<Movie> GetMovieById(long id)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Movies/{id}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Movie>(json);
        }

        public async Task<List<Movie>> SearchMovies(string query)
        {
            var allMovies = await GetAllMovies();
            if (string.IsNullOrEmpty(query)) return allMovies;

            return allMovies
                .Where(m => m.Name != null && m.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public async Task<List<MovieShowing>> GetShowingsForMovie(int movieId, DateTime? date = null)
        {
            string url = $"{_apiBaseUrl}/api/MovieShowing?movieId={movieId}";

            if (date.HasValue)
            {
                url += $"&date={date.Value:yyyy-MM-dd}";
            }

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new List<MovieShowing>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<MovieShowing>>(json) ?? new List<MovieShowing>();
        }


        public async Task<bool> UpdateMovie(Movie movie)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/api/Movies/{movie.Id}", movie);
            return response.IsSuccessStatusCode;
        }

        public async Task AddTicketsAsync(int userId, List<CartItem> cartItems)
        {
            foreach (var cartItem in cartItems)
            {
                foreach (var seat in cartItem.Seats)
                {

                    var ticket = new CreateTicket
                    {
                        UserId = userId,
                        MovieId = cartItem.MovieId,
                        ShowingId = cartItem.SelectedShowingId, 
                        SeatNumber = seat,
                        WatchMovie = cartItem.SelectedDate.Date + TimeSpan.Parse(cartItem.SelectedTime),
                        Amount =cartItem.Amount,
                        Price=cartItem.TotalPrice,
                        PaymentMethod = cartItem.PaymentMethod
                    };


                    var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/api/Ticket", ticket);
                    var content = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Ticket API failed: {response.StatusCode} - {content}");
                    }
                }
            }
        }

        public async Task<List<TicketViewModel>> GetTicketsByUserAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Ticket/GetTicketsForUser?userId={userId}");
            if (!response.IsSuccessStatusCode) return new List<TicketViewModel>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<TicketViewModel>>(json) ?? new List<TicketViewModel>();
        }

        public async Task<List<Seat>> GetSeatsForMovie(int showingId)
        {
            using var client = new HttpClient();
            var url = $"{_apiBaseUrl}/api/MovieShowing/seats?showingId={showingId}";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return new List<Seat>();
            }

            var json = await response.Content.ReadAsStringAsync();

            var seats = JsonConvert.DeserializeObject<List<Seat>>(json);
            return seats;
        }

        public async Task<List<string>> GetBookedSeatsForShowing(int showingId)
        {
            using var client = new HttpClient();
            var url = $"{_apiBaseUrl}/api/MovieShowing/bookedSeats?showingId={showingId}";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return new List<string>();
            }

            var json = await response.Content.ReadAsStringAsync();

            var bookedSeats = JsonConvert.DeserializeObject<List<string>>(json);
            return bookedSeats;
        }
        public async Task<bool> AddShowing(MovieShowing showing)
        {
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7054/api/MovieShowing", showing);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AddMovie(Movie movie)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Movie", movie);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveShowingAsync(int showingId)
        {
            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/api/MovieShowing/{showingId}");
            return response.IsSuccessStatusCode;
        }

    }
}
