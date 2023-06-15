namespace GraduationProject.BindingModels
{
    public enum SubmissionStatus:int
    {
        Accept=1,Wrong=2,TimeLimit=4,MemoryLimit=8,Compilationerror=16,Running=32,inQueue=64
    }
    public class OutputData4Submission
    {
        public string Result { get; set; }
        public long memory { get; set; }
        public decimal time { get; set; }
    }
}
