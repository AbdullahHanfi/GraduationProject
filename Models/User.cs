using GraduationProject.BindingModels;

namespace GraduationProject.Models;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public int RId { get; set; }

    public string UserName { get; set; }

    public string Name { get; set; }

    public byte IsValid { get; set; }

    public byte CheatTimes { get; set; }

    public virtual ICollection<Contest> Contests { get; set; } = new List<Contest>();

    public virtual Role RIdNavigation { get; set; }

    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();

    public virtual ICollection<Contest> CIds { get; set; } = new List<Contest>();
    public static explicit operator User(RegisterBinding item)
    {
        User user = new User();
        user.UserName = user.Name = item.Name;
        user.Email = item.Email;
        user.Password = item.Password;
        user.RId = 1;

        return user;
    }
}
