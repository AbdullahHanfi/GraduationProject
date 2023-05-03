
namespace GraduationProject.Models;

public partial class Language
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}
