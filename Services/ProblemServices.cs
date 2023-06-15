using GraduationProject.Models;

namespace GraduationProject.Services
{
    static public class ProblemServices
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="problems"></param>
        /// <param name="c_id"></param>
        /// <returns></returns>
        static public bool IsCreated(ref List<ProblemBinding> problems, int? c_id)
        {
            var db = new ProjectDbContext();
            List<Problem> problemsUploaded = new List<Problem>();

            foreach (var problem in problems)
            {

                Problem problemUploaded = new Problem()
                {
                    ProblemFile= problem.ProbelmFile,
                    Name = problem.Name,
                    TimeLimit = problem.Time_Limit,
                    MemoryLimit = problem.Memory_Limit,
                    CId = c_id ?? -1,
                    Visibility = true
                };
                problemsUploaded.Add(problemUploaded);
            }

            db.Problems.AddRange(problemsUploaded);

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
        #region Delete_problem
        //static public void DeleteTestCase(int P_ID)
        //{
        //    var db = new Gdb();
        //    var TestCases = db.Input_Cases.Where(www => www.Problem_ID == P_ID);
        //    foreach (var TestCase in TestCases)
        //    {
        //        TestCasesServices.DeleteTestCase(TestCase.ID);
        //    }
        //}
        //static public int DeleteProblem(int? P_ID, int? C_ID, ModelStateDictionary ModelState = default)
        //{
        //    Gdb db = new Gdb();

        //    if (C_ID is null || P_ID is null)
        //    {
        //        ModelState.AddModelError("", "PLZ LEAVE UR TESTING SKILLS OUT OF MY APP ☺️");
        //        ModelState.AddModelError("", "plz don't modfiy url or body");
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        return 0;
        //    }

        //    var ProblemData = db.Problems.FirstOrDefault(Problem => Problem.ID == P_ID && Problem.C_ID == C_ID);

        //    if (ProblemData is null)
        //    {
        //        ModelState.AddModelError("", "Not Found");
        //        return 0;
        //    }
        //    ProblemServices.DeleteTestCase(P_ID ?? -1);
        //    db.Problems.Remove(ProblemData);
        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch
        //    {
        //        return -1;
        //    }
        //    return 1;

        //}
        #endregion
    }
}