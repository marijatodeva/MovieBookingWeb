namespace MovieWeb.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string CardNumber { get; set; }
        public string CardHolder { get; set; }
        public string Expiry { get; set; }
        public string CVV { get; set; }
        public decimal Amount { get; set; }        
        public string PaymentMethod { get; set; }
        public string Seats { get; set; }     
        public DateTime PaidOn { get; set; } = DateTime.Now;
    }
}