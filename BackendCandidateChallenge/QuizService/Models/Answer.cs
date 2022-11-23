using System;
using System.Collections.Generic;

namespace QuizService.Models;

public partial class Answer
{
    public int Id { get; set; }

    public string Text { get; set; }

    public int QuestionId { get; set; }

    public virtual Question Question { get; set; }

    public virtual ICollection<Question> Questions { get; } = new List<Question>();
}
