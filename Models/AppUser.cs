using Newtonsoft.Json;

namespace MovieWeb.Models
{
    public class AppUser
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }

    }
}
