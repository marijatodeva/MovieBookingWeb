using Microsoft.Extensions.Options;
using MovieWeb.Models;
using MovieWeb.Models.System;
using Newtonsoft.Json;
using System.Text;

namespace MovieWeb.Services
{
    public class UserService : IUserService
    {
        private readonly string _apiBaseUrl;
        private readonly HttpClient _client;

        public UserService(IOptions<DBSettings> settings, IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
            _apiBaseUrl = settings.Value.DbApi ?? throw new ArgumentNullException(nameof(settings.Value.DbApi));
        }

        public async Task<LoginResponse> CheckUserCredidentals(LoginRequest loginRequest)
        {
            LoginResponse loginResponse = new LoginResponse();

            HttpContent requestContent = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");
            using (var response = await _client.PostAsync($"{_apiBaseUrl}/api/AppUser/Login", requestContent))
            {
                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    loginResponse = JsonConvert.DeserializeObject<LoginResponse>(apiResponse);
                }
                else
                {
                    return null;
                }
            }

            return loginResponse;
        }

        public async Task<bool> LogoutUser(string username)
        {
            var json = JsonConvert.SerializeObject(username);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{_apiBaseUrl}/api/AppUser/logout", content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> TryRegisterRequest(RegisterRequest registerRequest)
        {
            bool loginResponse;

            HttpContent requestContent = new StringContent(JsonConvert.SerializeObject(registerRequest), Encoding.UTF8, "application/json");
            using (var response = await _client.PostAsync($"{_apiBaseUrl}/api/AppUser/Register", requestContent))
            {
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        public async Task<IEnumerable<AppUser>> GetAllUsersAsync()
        {
            using var response = await _client.GetAsync($"{_apiBaseUrl}/api/AppUser");

            if (!response.IsSuccessStatusCode)
                return new List<AppUser>();

            var apiResponse = await response.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<IEnumerable<AppUser>>(apiResponse);

            return users;
        }

    }
}
