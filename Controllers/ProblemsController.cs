using GraduationProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProblemsController : ControllerBase
    {
        private readonly ProjectDbContext db;
        public ProblemsController()
        {
            db = new();
        }
        [HttpGet("{c_id:int}")]
        public async Task<IActionResult> Problems([FromRoute] int? c_id)
        {
            if (c_id is null)
                ModelState.AddModelError("", "PLZ LEAVE UR TESTING SKILLS OUT OF MY APP ☺️");
            if (!ModelState.IsValid)
            {
                var Errors = ModelState.Values.SelectMany(v => v.Errors);
                return StatusCode(422, Errors);
            }

            var problems =  db.Problems
                .Where(Problems => Problems.CId == c_id && (
                    Problems.Visibility ?? false))
                .Select(Problem => new
                {
                    ID = Problem.Id,
                    Time_Limit = Problem.TimeLimit,
                    Memory_Limit = Problem.MemoryLimit,
                    C_ID = Problem.CId,
                    Name = Problem.Name
                }).ToListAsync();

            return StatusCode((int)HttpStatusCode.OK, problems);
        }

        [HttpGet("{p_id:int}")]
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

            var Problem = (await db.Problems
                .FindAsync(p_id)
                );
            var contest = (await db.Contests
                .FindAsync(Problem.CId)
                );

            if (Problem is null || !(Problem.Visibility ?? false) || contest.StartAt.CompareTo(DateTime.Now) >= 0)
            {
                return StatusCode((int)HttpStatusCode.NoContent, "Not vlaid");
            }
            else
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Problems", Problem.FileName);
                if (!System.IO.File.Exists(filePath))
                    return NotFound();

                var memory = new MemoryStream();
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                return File(memory, $"*/*", Problem.FileName);
            }
        }

    }
}
