using GraduationProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GraduationProject.Services;
using System.Security.Claims;
using GraduationProject.BindingModels;

namespace GraduationProject.Controllers
{
    [Authorize(Roles = "Student")]
    public class SubmitController : ControllerBase
    {
        [HttpPost("Submit")]
        public void Submit(string? code, int? p_id, int? LangageId)
        {
            if (code != null && p_id != null && LangageId != null)
                using (var dbWR = new ProjectDbContext())
                {
                    var ProblemInfo = dbWR.Problems.Find(p_id);
                    var testCases = CompileSubmission.TestCases4Problem((int)p_id);
                    var submission = new Submission()
                    {
                        Code = code,
                        ProblemId = (int)p_id,
                        SubmissionTime = DateTime.Now,
                        LangageId = (int)LangageId,
                        UserId = Convert.ToInt32(User.FindFirstValue("ID")),
                        Memory=-1,
                        ExecutionTime=-1,
                        Status= (int)SubmissionStatus.Accept
                    };
                    if (testCases != null && ProblemInfo != null)
                    {
                        foreach (var testCase in testCases)
                        {
                            var CodeOutPut = CompileSubmission.ExecuteCppCode(
                                code,
                                System.IO.File.ReadAllText(Path.GetFullPath($"~/App_Data/TestCases/{testCase.InputCase}")),
                                ProblemInfo.TimeLimit,
                                ProblemInfo.MemoryLimit);

                            var check = CompileSubmission.CheckStatus(ref CodeOutPut, ref submission, testCase.OutputCase);
                            if (!check)
                            {
                                dbWR.Submissions.Add(submission);
                            }
                        }
                        dbWR.Submissions.Add(submission);
                    }

                }
        }

    }
}