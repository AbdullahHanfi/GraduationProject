using GraduationProject.BindingModels;
using GraduationProject.Models;
using GraduationProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace GraduationProject.Controllers
{
    //[Authorize(Roles = "SuperAdmin,Admin")]
    [ApiController]
    public class ManageContestController : ControllerBase
    {
        private readonly ProjectDbContext db;

        public ManageContestController()
        {
            db = new();
        }

        /// <summary>
        /// Use For Create One Contest 
        /// </summary>
        /// <returns>if data isn't Valid Return 409 . If Data is Valid 201 . If unknow will return 500</returns>
        [HttpPost("contest")]
        public async Task<ActionResult>  CreateContest([FromBody] ContestBinding contest)
        {
            int IsCreated = await ContestServices.IsCreated(contest, ModelState, HttpContext);
            if (IsCreated == 0)
            {
                var Errors = ModelState.Values.SelectMany(v => v.Errors);

                return StatusCode(
                    (int)HttpStatusCode.Conflict,
                    new { Errors, contest });
            }
            else if (IsCreated == -1)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, contest);
            }
            else
            {
                return StatusCode((int)HttpStatusCode.Created);
            }
        }
        /// <summary>
        /// Use For Return Data Of All Contest
        /// </summary>
        /// <returns>It Will be 200 in all time</returns>
        [HttpGet("contests")]
        //[AllowAnonymous]
        public ActionResult Contests()
        {
            var contests = db.Contests
                .Select(x => new
                {
                    ID = x.CId,
                    Name = x.Name,
                    End_in = x.EndIn,
                    Start_at = x.StartAt,
                    visibility = x.Visibility,
                    Number_of_Problem = db.Problems.Where(Problem => Problem.CId == x.CId).Count()
                }).ToList();

            return StatusCode((int)HttpStatusCode.OK, contests);
        }
        /// <summary>
        /// for get Data for specfy contest
        /// </summary>
        /// <returns>return 200 if data is found . if Data isn't found return 404</returns>
        [HttpGet("contest/{id:int}")]
        public async Task<ActionResult> Contest([FromRoute] int? id)
        {
            var Item = await db.Contests
                .FirstOrDefaultAsync(contest => contest.CId == id);

            if (!(Item is null))
            {

                return StatusCode((int)HttpStatusCode.OK, new
                {
                    ID = Item.CId,
                    Name = Item.Name,
                    End_in = Item.EndIn,
                    Start_at = Item.StartAt,
                    visibility = Item.Visibility
                });
            }
            else
                return StatusCode((int)HttpStatusCode.NotFound);
        }
        /// <summary>
        /// Use for edit contest
        /// </summary>
        /// <returns>if Data is Valid return 204 . if Data isn't Valid return 409 and Json file has Errors and Data . if unknow error return 500</returns>
        [HttpPut("contest/{id:int}")]
        public async Task<ActionResult> Contest([FromRoute] int? id, [FromBody] ContestBindingModel Item)
        {
            await ContestServices.ValidName(Item.Name, ModelState);
            await ContestServices.ValidDates(Item.Start_at, Item.End_in, ModelState);
            if (id == null || Item.ID == null || id != Item.ID)
            {
                ModelState.AddModelError("", "UnValid data");
            }
            if (!ModelState.IsValid)
            {
                var Errors = ModelState.Values.SelectMany(v => v.Errors);

                return StatusCode((int)HttpStatusCode.Conflict, new { Errors, Item });
            }
            int IsUpdated = await ContestServices.UpdateContest(Item);

            if (IsUpdated != 1)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            else
            {
                return StatusCode((int)HttpStatusCode.NoContent);
            }
        }
        /// <summary>
        /// Change State Contest
        /// </summary>
        /// <returns>return 204 if operation work corretly or 500 if server error</returns>
        [HttpDelete("contest/{id:int}")]
        public async Task<ActionResult> Contests([FromRoute] int? id)
        {
            ProjectDbContext dbWR = new ProjectDbContext();
            if (!(id is null))
            {
                var contest = dbWR.Contests.FirstOrDefault(cntest => cntest.CId == id);

                if (!(contest is null))
                {
                    contest.Visibility = !(contest.Visibility ?? true && true);
                    try
                    {
                        await dbWR.SaveChangesAsync();
                    }
                    catch
                    {
                        return StatusCode((int)HttpStatusCode.InternalServerError);
                    }
                }
            }
            return StatusCode((int)HttpStatusCode.NoContent);
        }
    }
}
