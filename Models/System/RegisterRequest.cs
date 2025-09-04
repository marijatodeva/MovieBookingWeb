using System.ComponentModel.DataAnnotations;

namespace MovieWeb.Models.System
{
    public class RegisterRequest
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

}
