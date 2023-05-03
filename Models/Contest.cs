
namespace GraduationProject.Models;

public partial class Contest
{
    public int CId { get; set; }

    public string Name { get; set; } = null!;

    public DateTime StartAt { get; set; }

    public DateTime EndIn { get; set; }

    public int AdminId { get; set; }

    public bool? Visibility { get; set; }

    public virtual User Admin { get; set; } = null!;

    public virtual ICollection<Problem> Problems { get; set; } = new List<Problem>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
