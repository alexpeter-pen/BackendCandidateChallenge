using System;
using System.Collections.Generic;

namespace QuizService.Models;

public partial class Quiz
{
    public int Id { get; set; }

    public string Title { get; set; }

    public virtual ICollection<Question> Questions { get; } = new List<Question>();
}
