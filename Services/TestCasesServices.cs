using GraduationProject.Models;

namespace GraduationProject.Services
{
    static public class TestCasesServices
    {
        static private bool Valid(ref List<TestCasesBinding> TestCases)
        {
            foreach (var TestCase in TestCases)
            {
                if (string.IsNullOrEmpty(TestCase.InputCase) || string.IsNullOrEmpty(TestCase.OutputCase))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// use for save Testcase
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="file">File as Base 64</param>
        static public void SaveTestCase(string filename, string file)
        {
            var pathInput = Path.GetFullPath($"~/App_Data/TestCases/{filename}");
            byte[] bytesInput = Convert.FromBase64String(file);
            File.WriteAllBytes(pathInput, bytesInput);
        }
        static public bool IsCreated(ref List<TestCasesBinding> TestCases, int? p_id)
        {
            ProjectDbContext db = new ProjectDbContext();
            List<InputCase> Inputcases = new List<InputCase>();
            List<OutputCase> Outputcases = new List<OutputCase>();
            var IsValid = Valid(ref TestCases);
            if (!IsValid)
                return false;
            foreach (var TestCase in TestCases)
            {

                InputCase Inputcase = new InputCase()
                {
                    Id = Guid.NewGuid().ToString(),
                    ProblemId = p_id ?? -1
                };
                OutputCase Outputcase = new OutputCase()
                {
                    Id = Guid.NewGuid().ToString(),
                    InputId = Inputcase.Id
                };

                SaveTestCase(Inputcase.Id, TestCase.InputCase);

                SaveTestCase(Outputcase.Id, TestCase.OutputCase);

                Inputcases.Add(Inputcase);
                Outputcases.Add(Outputcase);
            }

            db.InputCases.AddRange(Inputcases);
            db.OutputCases.AddRange(Outputcases);
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