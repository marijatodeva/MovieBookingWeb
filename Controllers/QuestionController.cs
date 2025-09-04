using Microsoft.AspNetCore.Mvc;
using MovieWeb.Services;
using System.Linq;
using System.Threading.Tasks;

namespace MovieWeb.Controllers
{
    [Route("Question")]
    public class QuestionController : Controller
    {
        private readonly IQuestionService _service;

        public QuestionController(IQuestionService service)
        {
            _service = service;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var questions = await _service.GetAllQuestionsAsync();
            return View(questions);
        }

        [HttpGet("Answer")]
        public async Task<IActionResult> GetAnswer(string questionText)
        {
            var questions = await _service.GetAllQuestionsAsync();
            var answer = questions.FirstOrDefault(q => q.QuestionText.ToLower().Contains(questionText.ToLower()));
            if (answer != null)
                return Ok(answer.AnswerText);

            return Ok("Sorry, I don't know the answer to that question.");
        }
    }
}
