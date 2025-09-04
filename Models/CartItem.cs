namespace MovieWeb.Models
{
    public class CartItem
    {
        public int MovieId { get; set; }
        public string MovieName { get; set; }
        public string ImageUrl { get; set; }
        public List<string> Seats { get; set; } = new List<string>();
        public DateTime SelectedDate { get; set; }
        public int Amount { get; set; }
        public decimal PricePerTicket { get; set; }
        public decimal TotalPrice => Amount * PricePerTicket;
        public string SelectedTime { get; set; }
        public string PaymentMethod { get; set; }
        public int SelectedShowingId { get; set; } 

    }
}
