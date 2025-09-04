using MovieWeb.Models;
using MovieWeb.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieWeb.Services
{
    public interface IQuestionService
    {
        Task<IEnumerable<Question>> GetAllQuestionsAsync();
        Task<Question> GetQuestionByIdAsync(int id);
    }
}
