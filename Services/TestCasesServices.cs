using GraduationProject.Models;

namespace GraduationProject.Services
{
    static public class TestCasesServices
    {

        static public bool IsCreated(ref TestCasesBinding TestCase, int? p_id)
        {
            ProjectDbContext db = new ProjectDbContext();

            using var readerinput = new StreamReader(TestCase.InputCase.OpenReadStream());
            using var readeroutput = new StreamReader(TestCase.OutputCase.OpenReadStream());

            InputCase Inputcase = new InputCase()
            {
                Id = Guid.NewGuid().ToString(),
                ProblemId = p_id ?? -1,
                Input = readerinput.ReadToEnd()
            };
            OutputCase Outputcase = new OutputCase()
            {
                Id = Guid.NewGuid().ToString(),
                InputId = Inputcase.Id,
                Output = readeroutput.ReadToEnd()
            };

            db.InputCases.Add(Inputcase);
            db.OutputCases.Add(Outputcase);
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="TestID"></param>
        /// <returns>valid operation 1 . not valid data 0 . server error -1</returns>
        public static int DeleteTestCase(string TestID)
        {
            var db = new ProjectDbContext();
            if (string.IsNullOrEmpty(TestID))
                return 0;

            var InputCase = db.InputCases.FirstOrDefault(x => x.Id == TestID);

            if (InputCase is null)
            {
                var OutputCase = db.OutputCases.FirstOrDefault(x => x.Id == TestID);
                if (OutputCase is null)
                    return 0;

                var RelatedInputCase = db.InputCases.Find(OutputCase.InputId);
                if (!(RelatedInputCase is null))
                    db.InputCases.Remove(RelatedInputCase);
                db.OutputCases.Remove(OutputCase);
            }
            else
            {
                var RelatedOutputCase = db.OutputCases.FirstOrDefault(www => www.InputId == InputCase.Id);
                if (!(RelatedOutputCase is null))
                    db.OutputCases.Remove(RelatedOutputCase);
                db.InputCases.Remove(InputCase);
            }
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return -1;
            }
            return 1;
        }
    }
}