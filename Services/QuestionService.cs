using Microsoft.Extensions.Options;
using MovieWeb.Models;
using MovieWeb.Models.System;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MovieWeb.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public QuestionService(IOptions<DBSettings> settings)
        {
            _httpClient = new HttpClient();
            _apiBaseUrl = settings.Value.DbApi ?? throw new ArgumentNullException(nameof(settings.Value.DbApi));
        }

        public async Task<IEnumerable<Question>> GetAllQuestionsAsync()
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Question");
            if (!response.IsSuccessStatusCode)
                return new List<Question>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<Question>>(json) ?? new List<Question>();
        }

        public async Task<Question?> GetQuestionByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Question/{id}");
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Question>(json);
        }
    }
}
