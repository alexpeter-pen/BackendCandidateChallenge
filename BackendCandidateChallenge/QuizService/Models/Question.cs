using System;
using System.Collections.Generic;

namespace QuizService.Models;

public partial class Question
{
    public int Id { get; set; }

    public string Text { get; set; }

    public int QuizId { get; set; }

    public int? CorrectAnswerId { get; set; }

    public virtual ICollection<Answer> Answers { get; } = new List<Answer>();

    public virtual Answer CorrectAnswer { get; set; }

    public virtual Quiz Quiz { get; set; }
}
