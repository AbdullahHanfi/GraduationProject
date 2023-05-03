using GraduationProject.BindingModels;
using GraduationProject.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace GraduationProject.Services
{
    static public class ContestServices
    {
        /// <summary>
        /// For check Contest Name
        /// </summary>
        /// <returns>if Name is Valid return One else Zero</returns>
        static public async Task<bool> ValidName(string Name, ModelStateDictionary ModelState = default)
        {
            ProjectDbContext db = new ProjectDbContext();
            bool IsValid = db.Contests.Where(ww => ww.Name == Name).Any();
            if (IsValid && ModelState != default)
            {
                ModelState.AddModelError("Name", "Not Valid Name");
            }
            return !IsValid;
        }
        /// <summary>
        /// For check Contest Dates
        /// </summary>
        /// <returns>if Date is Valid return One else Zero</returns>
        static public async Task<bool> ValidDates(DateTime Start_at, DateTime End_in, ModelStateDictionary ModelState = default)
        {
            ProjectDbContext db = new ProjectDbContext();
            bool IsValid = DateTime.Compare(Start_at, End_in) == 0 &&
                         DateTime.Compare(DateTime.Now, Start_at) == 0;
            if (IsValid && ModelState != default)
            {
                ModelState.AddModelError("", "Not Valid Dates");
            }

            return !IsValid;
        }

        /// <summary>
        /// Create Contest
        /// </summary>
        /// <returns>return One if Contest Data is Valid or Zero if Contest Data isn't Valid or -1 if unknow Error</returns>
        static public async Task<int> IsCreated(ContestBinding contest, ModelStateDictionary ModelState, HttpContext HttpContext)
        {
            await ContestServices.ValidName(contest.Name, ModelState);
            await ContestServices.ValidDates(contest.Start_at, contest.End_in, ModelState);

            if (!ModelState.IsValid)
            {
                return 0;
            }
            else
            {
                ProjectDbContext db = new ProjectDbContext();

                Contest _contest = new Contest();
                _contest.Name = contest.Name;
                _contest.StartAt = contest.Start_at;
                _contest.EndIn = contest.End_in;
                _contest.AdminId = Convert.ToInt32(HttpContext.User.FindFirstValue("ID"));
                _contest.Visibility = true;

                db.Contests.Add(_contest);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    return -1;
                }
                return 1;
            }
        }
        /// <summary>
        /// Update Contest
        /// </summary>
        /// <returns>return One if Contest Data is Update or Zero if Contest Data isn't Uodate or -1 if unknow Error</returns>
        static public async Task<int> UpdateContest(ContestBindingModel Item)
        {
            using (ProjectDbContext db = new ProjectDbContext())
            {
                var result = await db.Contests.SingleOrDefaultAsync(b => b.CId == Item.ID);
                if (result != null)
                {
                    result.StartAt = Item.Start_at;
                    result.EndIn = Item.End_in;
                    result.Name = Item.Name;
                    try
                    {
                        await db.SaveChangesAsync();
                    }
                    catch (Exception)
                    {
                        return -1;
                    }
                    return 1;
                }
                return 0;
            }
        }

        //public static void DeleteProlem(int ContestID)
        //{
        //    Gdb db = new Gdb();
        //    var Problems = db.Problems.Where(b => b.C_ID == ContestID);
        //    foreach (var Problem in Problems)
        //    {
        //        ProblemServices.DeleteProblem(Problem.ID, Problem.C_ID);
        //    }
        //}

    }
}