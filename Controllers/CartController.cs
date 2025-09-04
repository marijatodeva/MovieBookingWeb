using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MovieWeb.Models;
using MovieWeb.Models.System;
using MovieWeb.Services;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieWeb.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private const string SessionCartKey = "Cart";
        private readonly IMovieService _movieService;

        public CartController(IOptions<DBSettings> settings, IMovieService movieService)
        {
            _movieService = movieService;
        }

        private List<CartItem> GetCart()
        {
            var cartJson = HttpContext.Session.GetString(SessionCartKey);
            return cartJson == null ? new List<CartItem>() : JsonConvert.DeserializeObject<List<CartItem>>(cartJson);
        }

        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetString(SessionCartKey, JsonConvert.SerializeObject(cart));
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(CartItem item)
        {
            var cart = GetCart();
            var movie = await _movieService.GetMovieById(item.MovieId);
            if (movie == null) return NotFound();

            item.MovieName = movie.Name;
            item.ImageUrl = movie.ImageUrl;
            item.PricePerTicket = movie.Price;
            item.Amount = item.Seats?.Count ?? 1;

            var showings = await _movieService.GetShowingsForMovie(item.MovieId, item.SelectedDate);

            var showing = showings.FirstOrDefault(s => s.Id == item.SelectedShowingId);

            if (showing != null)
            {
                item.SelectedTime = showing.ShowingTime;   
                item.SelectedShowingId = showing.Id;
                item.SelectedDate = showing.ShowingDate;
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid showing selected.";
                return RedirectToAction("Ticket", new { movieId = item.MovieId });
            }

            var existing = cart.FirstOrDefault(c =>
                c.MovieId == item.MovieId &&
                c.SelectedDate == item.SelectedDate &&
                c.SelectedTime == item.SelectedTime);

            if (existing != null)
            {
                foreach (var seat in item.Seats)
                    if (!existing.Seats.Contains(seat))
                        existing.Seats.Add(seat);

                existing.Amount = existing.Seats.Count;
            }
            else
            {
                cart.Add(item);
            }

            SaveCart(cart);
            TempData["SuccessMessage"] = $"{item.MovieName} added to cart!";
            return RedirectToAction("Cart");
        }

        public IActionResult Cart()
        {
            var cart = GetCart();
            return View(cart);
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int movieId, DateTime selectedDate, string selectedTime, string seats, string action)
        {
            var cart = GetCart();
            var seatList = seats.Split(',').ToList();

            var item = cart.FirstOrDefault(c =>
                c.MovieId == movieId &&
                c.SelectedDate == selectedDate &&
                c.SelectedTime == selectedTime &&
                c.Seats.SequenceEqual(seatList));

            if (item != null)
            {
                if (action == "decrease" && item.Seats.Count > 1)
                    item.Seats.RemoveAt(item.Seats.Count - 1); 
                else if (action == "increase")
                {
                    item.Seats.Add("X" + (item.Seats.Count + 1));
                }

                item.Amount = item.Seats.Count;

                if (item.Amount <= 0)
                    cart.Remove(item);
            }
            SaveCart(cart);
            return RedirectToAction("Cart");
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int movieId, DateTime selectedDate, string selectedTime, string seats)
        {
            if (string.IsNullOrEmpty(seats))
            {
                TempData["ErrorMessage"] = "No seats selected to remove.";
                return RedirectToAction("Cart");
            }

            var cart = GetCart();
            var seatList = seats.Split(',').ToList();

            var item = cart.FirstOrDefault(c =>
                c.MovieId == movieId &&
                c.SelectedDate == selectedDate &&
                c.SelectedTime == selectedTime &&
                c.Seats.SequenceEqual(seatList));

            if (item != null)
                cart.Remove(item);

            SaveCart(cart);
            return RedirectToAction("Cart");
        }


        public IActionResult ClearCart()
        {
            SaveCart(new List<CartItem>());
            TempData["SuccessMessage"] = "Cart cleared!";
            return RedirectToAction("Cart");
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(string paymentMethod)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                TempData["ErrorMessage"] = "You must be logged in to book tickets!";
                return RedirectToAction("Login", "AppUser" );
            }

            int userId = int.Parse(userIdClaim.Value);

            var cart = GetCart();
            if (cart == null || !cart.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty!";
                return RedirectToAction("Cart");
            }

            foreach (var item in cart)
                item.PaymentMethod = paymentMethod;

            await _movieService.AddTicketsAsync(userId, cart);

            SaveCart(new List<CartItem>());

            TempData["SuccessMessage"] = "Payment successful! Your tickets are booked.";

            return RedirectToAction("Index", "Ticket");
        }


        [HttpGet]
        public IActionResult CreditCardForm(string paymentMethod)
        {
            var cart = GetCart();
            if (cart == null || !cart.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty!";
                return RedirectToAction("Cart");
            }

            var model = new CreditCardPaymentModel
            {
                PaymentMethod = paymentMethod,
                MovieId = cart.First().MovieId,
                WatchMovie = cart.First().SelectedDate,
                Seats = cart.First().Seats.ToArray()
            };

            return View(model); 
        }
    }
}
