namespace MovieWeb.Models
{
    public class Ticket
    {
        public long Id { get; set; }
        public int Amount { get; set; } 
        public DateTime WatchMovie { get; set; } 
        public long MovieId { get; set; } 
        public long UserId { get; set; }
        public string PaymentMethod { get; set; }
        public string UserName { get; set; }
        public string SeatNumber { get; set; }
        public decimal Price {  get; set; }
        public int ShowingId { get; set; }  

    }

    public class CreateTicket
    {
        public int Amount { get; set; }
        public DateTime WatchMovie { get; set; }
        public long UserId { get; set; }
        public long MovieId { get; set; }
        public string PaymentMethod { get; set; }
        public string SeatNumber { get; set; }
        public decimal Price { get; set; }
        public int ShowingId { get; set; }
    }
}
