using GraduationProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GraduationProject.Services;
using System.Security.Claims;
using GraduationProject.BindingModels;
using System.ComponentModel.DataAnnotations;

namespace GraduationProject.Controllers
{
    //[Authorize(Roles = "Student")]
    public class SubmitController : ControllerBase
    {
        [HttpPost("Submit")]
        public async Task Submit([FromBody] SubmitInfo info)
        {
            if (!string.IsNullOrWhiteSpace(info.code) && info.p_id != null)
                using (var dbWR = new ProjectDbContext())
                {
                    var ProblemInfo = dbWR.Problems.Find(info.p_id);
                    var testCases = CompileSubmission.TestCases4Problem((int)info.p_id);
                    var submission = new Submission()
                    {
                        Code = info.code,
                        ProblemId = (int)info.p_id,
                        SubmissionTime = DateTime.Now,
                        LangageId = 1,
                        UserId = 0, /*Convert.ToInt32(User.FindFirstValue("ID"))*/
                        Memory = -1,
                        ExecutionTime = -1,
                        Status = (int)SubmissionStatus.inQueue
                    };
                    dbWR.Submissions.Add(submission);
                    dbWR.SaveChanges();

                    if (testCases != null && ProblemInfo != null)
                    {
                        submission.Status = (int)SubmissionStatus.Running;
                        foreach (var testCase in testCases)
                        {
                            var CodeOutPut = CompileSubmission.ExecuteCppCode(info.code,
                                testCase.InputCase,
                                ProblemInfo.TimeLimit,
                                ProblemInfo.MemoryLimit);

                            var check = CompileSubmission.CheckStatus(ref CodeOutPut, ref submission, testCase.OutputCase);
                            if (!check)
                            {
                                dbWR.Submissions.Add(submission);
                            }
                        }
                        if (submission.Status == (int)SubmissionStatus.Running)
                            submission.Status = (int)SubmissionStatus.Accept;
                        dbWR.Submissions.Update(submission);
                        dbWR.SaveChanges();
                    }

                }
        }

    }
}