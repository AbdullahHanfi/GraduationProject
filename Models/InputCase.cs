using System;
using System.Collections.Generic;

namespace GraduationProject.Models;

public partial class InputCase
{
    public string Id { get; set; }

    public int ProblemId { get; set; }

    public virtual ICollection<OutputCase> OutputCases { get; set; } = new List<OutputCase>();

    public virtual Problem Problem { get; set; }
}
