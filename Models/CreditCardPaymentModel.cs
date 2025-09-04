namespace MovieWeb.Models
{
    public class CreditCardPaymentModel
    {
        public long MovieId { get; set; }
        public DateTime WatchMovie { get; set; }
        public string[] Seats { get; set; }
        public decimal Amount { get; set; }         
        public string CardNumber { get; set; }
        public string CardHolderName { get; set; }
        public string Expiration { get; set; } 
        public string CVV { get; set; }
        public string PaymentMethod { get; set; }
        public int SelectedShowingId { get; set; }
    }
}
