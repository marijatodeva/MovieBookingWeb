using System;

namespace MovieWeb.Models
    {
        public class Question
        {
            public int Id { get; set; }
            public string QuestionText { get; set; }
            public string AnswerText { get; set; }
            public string Category { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.Now;
        }
    }

