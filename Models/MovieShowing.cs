namespace MovieWeb.Models
{
    public class MovieShowing
    {
        public int Id { get; set; }                
        public int MovieId { get; set; }           
        public DateTime ShowingDate { get; set; }  
        public string ShowingTime { get; set; }
        public int HallId { get; set; }
    }
    public class CreateMovieShowing
    {
        public int MovieId { get; set; }
        public DateTime ShowingDate { get; set; }
        public string ShowingTime { get; set; }
        public int HallId { get; set; }
    }
}
