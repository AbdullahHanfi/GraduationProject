using System;
using System.Collections.Generic;

namespace GraduationProject.Models;

public partial class OutputCase
{
    public string Id { get; set; }

    public string InputId { get; set; }

    public string Output { get; set; }

    public virtual InputCase Input { get; set; }
}
