using Newtonsoft.Json;

namespace MovieWeb.Models.System
{
    public class LoginResponse
    {
        [JsonProperty("accessToken")]
        public string Token { get; set; }

        [JsonProperty("user")]
        public AppUser User { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }
    }
}
