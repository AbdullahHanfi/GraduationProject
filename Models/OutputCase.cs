
namespace GraduationProject.Models;

public partial class OutputCase
{
    public string Id { get; set; } = null!;

    public string InputId { get; set; } = null!;

    public virtual InputCase Input { get; set; } = null!;
}
