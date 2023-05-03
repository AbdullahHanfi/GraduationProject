using Microsoft.EntityFrameworkCore;

namespace GraduationProject.Models;

public partial class ProjectDbContext : DbContext
{
    public ProjectDbContext()
    {
    }

    public ProjectDbContext(DbContextOptions<ProjectDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Contest> Contests { get; set; }

    public virtual DbSet<InputCase> InputCases { get; set; }

    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<OutputCase> OutputCases { get; set; }

    public virtual DbSet<Problem> Problems { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Submission> Submissions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=Project_DB;Integrated Security=True;TrustServerCertificate=True;Encrypt=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contest>(entity =>
        {
            entity.HasKey(e => e.CId);

            entity.ToTable("Contest");

            entity.Property(e => e.CId).HasColumnName("C_ID");
            entity.Property(e => e.AdminId).HasColumnName("Admin_ID");
            entity.Property(e => e.EndIn)
                .HasColumnType("smalldatetime")
                .HasColumnName("End_in");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.StartAt)
                .HasColumnType("smalldatetime")
                .HasColumnName("Start_at");
            entity.Property(e => e.Visibility)
                .IsRequired()
                .HasDefaultValueSql("((1))")
                .HasColumnName("visibility");

            entity.HasOne(d => d.Admin).WithMany(p => p.Contests)
                .HasForeignKey(d => d.AdminId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Contest_User");

            entity.HasMany(d => d.Users).WithMany(p => p.CIds)
                .UsingEntity<Dictionary<string, object>>(
                    "Participant",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Participant_User"),
                    l => l.HasOne<Contest>().WithMany()
                        .HasForeignKey("CId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Participant_Contest"),
                    j =>
                    {
                        j.HasKey("CId", "UserId");
                        j.ToTable("Participant");
                        j.IndexerProperty<int>("CId").HasColumnName("C_ID");
                        j.IndexerProperty<int>("UserId").HasColumnName("User_ID");
                    });
        });

        modelBuilder.Entity<InputCase>(entity =>
        {
            entity.ToTable("Input_Cases");

            entity.Property(e => e.Id)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("ID");
            entity.Property(e => e.ProblemId)
                .ValueGeneratedOnAdd()
                .HasColumnName("Problem_ID");

            entity.HasOne(d => d.Problem).WithMany(p => p.InputCases)
                .HasForeignKey(d => d.ProblemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Input_Cases_Problem");
        });

        modelBuilder.Entity<Language>(entity =>
        {
            entity.ToTable("Language");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<OutputCase>(entity =>
        {
            entity.ToTable("Output_Cases");

            entity.Property(e => e.Id)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ID");
            entity.Property(e => e.InputId)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("Input_ID");

            entity.HasOne(d => d.Input).WithMany(p => p.OutputCases)
                .HasForeignKey(d => d.InputId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Output_Cases_Input_Cases1");
        });

        modelBuilder.Entity<Problem>(entity =>
        {
            entity.ToTable("Problem");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CId).HasColumnName("C_ID");
            entity.Property(e => e.MemoryLimit).HasColumnName("Memory_Limit");
            entity.Property(e => e.Name).IsUnicode(false);
            entity.Property(e => e.TimeLimit)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("Time_Limit");
            entity.Property(e => e.Visibility)
                .IsRequired()
                .HasDefaultValueSql("((1))")
                .HasColumnName("visibility");

            entity.HasOne(d => d.CIdNavigation).WithMany(p => p.Problems)
                .HasForeignKey(d => d.CId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Problem_Contest");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Submission>(entity =>
        {
            entity.HasKey(e => new { e.ProblemId, e.UserId });

            entity.ToTable("Submission");

            entity.Property(e => e.ProblemId).HasColumnName("Problem_ID");
            entity.Property(e => e.UserId).HasColumnName("User_ID");
            entity.Property(e => e.Code).IsUnicode(false);
            entity.Property(e => e.ExecutionTime).HasColumnName("Execution_Time");
            entity.Property(e => e.LangageId).HasColumnName("Langage_ID");
            entity.Property(e => e.SubmissionTime)
                .HasColumnType("smalldatetime")
                .HasColumnName("Submission_Time");

            entity.HasOne(d => d.Langage).WithMany(p => p.Submissions)
                .HasForeignKey(d => d.LangageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Submission_Language");

            entity.HasOne(d => d.Problem).WithMany(p => p.Submissions)
                .HasForeignKey(d => d.ProblemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Submission_Problem");

            entity.HasOne(d => d.User).WithMany(p => p.Submissions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Submission_User");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.IsValid).HasDefaultValueSql("((1))");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.RId).HasColumnName("R_ID");
            entity.Property(e => e.UserName)
                .HasMaxLength(64)
                .IsUnicode(false);

            entity.HasOne(d => d.RIdNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.RId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
