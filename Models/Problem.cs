using System;
using System.Collections.Generic;

namespace GraduationProject.Models;

public partial class Problem
{
    public int Id { get; set; }

    public decimal TimeLimit { get; set; }

    public int MemoryLimit { get; set; }

    public int CId { get; set; }

    public bool? Visibility { get; set; }

    public string Name { get; set; }

    public string FileName { get; set; }

    public virtual Contest CIdNavigation { get; set; }

    public virtual ICollection<InputCase> InputCases { get; set; } = new List<InputCase>();

    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}
