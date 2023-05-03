namespace GraduationProject.BindingModels
{
    public enum SubmissionStatus
    {
        Accept,Wrong,TimeLimit,MemoryLimit,Compilationerror
    }
    public class OutputData4Submission
    {
        public string Result { get; set; }
        public long memory { get; set; }
        public decimal time { get; set; }
    }
}
