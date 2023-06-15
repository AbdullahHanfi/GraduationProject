using GraduationProject.Models;
using GraduationProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace GraduationProject.Controllers
{
    //[Authorize(Roles = "SuperAdmin,Admin")]
    public class ManageProblemController : ControllerBase
    {
        private readonly ProjectDbContext db;
        public ManageProblemController()
        {
            db = new();
        }
        /// <summary>
        /// For add Problems in Contest
        /// </summary>
        /// <param name="c_id">Contest ID</param>
        /// <param name="problems">List of Problem Data</param>
        /// <returns></returns>
        
        [HttpPost("contest/{c_id:int}/problems")]
        public ActionResult Prolem([FromRoute] int? c_id, [FromBody] List<ProblemBinding> problems)
        {
            if (c_id is null || problems is null)
                ModelState.AddModelError("", "not valid data");

            if (!ModelState.IsValid)
            {
                var Errors = ModelState.Values.SelectMany(v => v.Errors);

                return StatusCode(422, Errors);
            }

            bool IsCreated = ProblemServices.IsCreated(ref problems, c_id);
            if (IsCreated)
            {
                return StatusCode((int)HttpStatusCode.Created);
            }
            else
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
        /// <summary>
        /// For Get All problems in contest without problemfile
        /// </summary>
        /// <param name="c_id">Contest ID</param>
        /// <returns>if Data isn't Valid return 422 . if data Valid return 200 and Json file With Problem Data without problemfile</returns>
        [HttpGet("contest/{c_id:int}/problems")]

        public ActionResult Problems([FromRoute] int? c_id)
        {
            if (c_id is null)
                ModelState.AddModelError("", "PLZ LEAVE UR TESTING SKILLS OUT OF MY APP ☺️");
            if (!ModelState.IsValid)
            {
                var Errors = ModelState.Values.SelectMany(v => v.Errors);
                return StatusCode(422, Errors);
            }

            var problems = db.Problems
                .Where(contest => contest.CId == c_id)
                .Select(Problem => new
                {
                    ID = Problem.Id,
                    Time_Limit = Problem.TimeLimit,
                    Memory_Limit = Problem.MemoryLimit,
                    C_ID = Problem.CId,
                    visibility = Problem.Visibility
                }).ToList();

            return StatusCode((int)HttpStatusCode.OK, problems);
        }
        /// <summary>
        /// Get File of problem
        /// </summary>
        /// <param name="p_id">Problem ID</param>
        /// <returns>if Data isn't Valid return 422 . if data Valid return 200 and file for user</returns>
        [HttpGet("problem/{p_id:int}")]
        public async Task<IActionResult> Problem([FromRoute] int? p_id)
        {
            if (p_id is null)
            {
                ModelState.AddModelError("", "plz don't modfiy url ");
            }

            if (!ModelState.IsValid)
            {
                var Errors = ModelState.Values.SelectMany(v => v.Errors);
                return StatusCode(422, Errors);
            }

            var ProblemFile = (await db.Problems
                .FirstOrDefaultAsync(Problem =>
                 Problem.Id == p_id )
                )?.Name;
            
            if (ProblemFile is null)
            {
                return StatusCode((int)HttpStatusCode.NoContent, "Not vlaid");
            }
            else
            {
                return File(
                    Convert.FromBase64String(ProblemFile),
                     "application/octet-stream"
                    );
            }
        }
        /// <summary>
        /// change state for problem 
        /// </summary>
        /// <param name="p_id">Problem ID</param>
        /// <returns>it return 204 if problem deleted . if Data isn't Valid return 422 and json file for error . if server error 500</returns>
        [HttpPut("problem/{p_id:int}")]
        public ActionResult DeleteProblem([FromRoute] int? p_id)
        {
            ProjectDbContext dbWR = new ProjectDbContext();
            if (!(p_id is null))
            {
                var problem = dbWR.Problems.Find(p_id);

                if (!(problem is null))
                {
                    problem.Visibility ^= true;
                    try
                    {
                        dbWR.SaveChanges();
                    }
                    catch
                    {
                        return StatusCode((int)HttpStatusCode.InternalServerError);
                    }
                }
            }
            return StatusCode((int)HttpStatusCode.NoContent,"Not Found");
        }
    }
}
