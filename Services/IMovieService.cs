using MovieWeb.Models;

namespace MovieWeb.Services
{
    public interface IMovieService
    {
        Task<List<Movie>> GetAllMovies();
        Task<Movie> GetMovieById(long id);
        Task<List<Movie>> SearchMovies(string query);
        Task<bool> UpdateMovie(Movie movie);
        Task<List<MovieShowing>> GetShowingsForMovie(int movieId, DateTime? date = null);
        Task AddTicketsAsync(int userId, List<CartItem> cartItems);
        Task<List<TicketViewModel>> GetTicketsByUserAsync(int userId);
        Task<List<Seat>> GetSeatsForMovie(int movieId);
        Task<List<string>> GetBookedSeatsForShowing(int showingId);
        Task<bool> AddShowing(MovieShowing showing);
        Task<bool> AddMovie(Movie movie);
        Task<bool> RemoveShowingAsync(int showingId);
    }
}
