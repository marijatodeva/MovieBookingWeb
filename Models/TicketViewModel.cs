namespace MovieWeb.Models
{
    public class TicketViewModel
    {
        public long Id { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
        public DateTime WatchMovie { get; set; }
        public string AppUserId { get; set; }
        public string MovieName { get; set; }
        public string PaymentMethod { get; set; }
        public string UserName { get; set; }
        public string SeatNumber { get; set; }
        public int SelectedShowingId { get; set; }
    }

    public class TicketPageViewModel
    {
        public Movie Movie { get; set; }
        public List<SeatViewModel> Seats { get; set; } = new();
        public List<MovieShowing> Showings { get; set; }
        public DateTime? SelectedDate { get; set; } = DateTime.Today;
        public int? SelectedShowingId { get; set; }
        public int HallId { get; set; }

    }
}