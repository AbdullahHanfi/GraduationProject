using System.ComponentModel.DataAnnotations;

namespace GraduationProject.Models
{
    public class ProblemBinding
    {
        [Required]
        public IFormFile? ProbelmFile { get; set; }
        [Required]
        public decimal? Time_Limit { get; set; }
        [Required]
        public int? Memory_Limit { get; set; }
    }
}