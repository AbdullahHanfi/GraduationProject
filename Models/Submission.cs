using System;
using System.Collections.Generic;

namespace GraduationProject.Models;

public partial class Submission
{
    public int ProblemId { get; set; }

    public int UserId { get; set; }

    public string Code { get; set; }

    public byte Status { get; set; }

    public int Memory { get; set; }

    public double ExecutionTime { get; set; }

    public int LangageId { get; set; }

    public DateTime SubmissionTime { get; set; }

    public virtual Language Langage { get; set; }

    public virtual Problem Problem { get; set; }

    public virtual User User { get; set; }
}
