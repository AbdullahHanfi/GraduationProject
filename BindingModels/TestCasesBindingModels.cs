using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GraduationProject.Models
{
    public class TestCasesBinding
    {
        public IFormFile? InputCase { get; set; }
        public IFormFile? OutputCase { get; set; }

    }
    public class TestCasesworkmodel
    {
        public string InputCase { get; set; }
        public string OutputCase { get; set; }

    }

}