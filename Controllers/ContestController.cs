using GraduationProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "SuperAdmin,Admin,Student")]
    public class ContestController : ControllerBase
    {
        private readonly ProjectDbContext db;

        public ContestController()
        {
            db = new();
        }
        [HttpGet("")]
        public ActionResult GetContests()
        {
            var contests = db.Contests
                .Select(x => new
                {
                    ID = x.CId,
                    Name = x.Name,
                    End_in = x.EndIn,
                    Start_at = x.StartAt,
                    Number_of_Problem = db.Problems.Where(Problem => Problem.CId == x.CId && (Problem.Visibility ?? false)).Count()
                }).ToList();

            return StatusCode((int)HttpStatusCode.OK, contests);
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetContest([FromRoute] int? id)
        {
            var Item = await db.Contests
                .FirstOrDefaultAsync(contest => contest.CId == id && (contest.Visibility ?? false));

            if (!(Item is null))
            {
                return StatusCode((int)HttpStatusCode.OK, new
                {
                    ID = Item.CId,
                    Name = Item.Name,
                    End_in = Item.EndIn,
                    Start_at = Item.StartAt
                });
            }
            else
                return StatusCode((int)HttpStatusCode.NotFound);
        }

        [HttpPut("{id:int}")]
        public async Task PutContest([FromRoute] int? id)
        {
            using (var DBWR = new ProjectDbContext())
            {
            }
        }


    }
}
