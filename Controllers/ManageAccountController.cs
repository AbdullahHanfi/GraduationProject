using GraduationProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GraduationProject.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class ManageAccountController : ControllerBase
    {
        private readonly ProjectDbContext db;
        ManageAccountController()
        {
            db = new ProjectDbContext();
        }
        /// <summary>
        /// Get all Accounts for Super Admin
        /// </summary>
        /// <returns>List of Accounts</returns>
        [HttpGet("accounts")]
        public ActionResult GetAccounts()
        {
            var Accounts = from Users in db.Users
                           join Roles in db.Roles on Users.RId equals Roles.Id
                           where Roles.Id != 1
                           select new
                           {
                               ID = Users.Id,
                               Name = Users.Name,
                               Email = Users.Email,
                               Role = Roles.Name,
                               IsValid = Users.IsValid
                           };
            #region EFQuery
            //    db.Users
            //.Join(
            //    db.Roles,
            //    Users => Users.R_ID,
            //    Roles => Roles.ID,
            //(Users, Roles) => new
            //{
            //ID = Users.id,
            //    Name = Users.Name,
            //    Email = Users.Email,
            //    Role = Roles.Name,
            //    IsValid = Users.IsValid
            //}).ToList();
            #endregion

            return Ok(Accounts);
        }

        [HttpGet("account/changestate/{id:int}")]
        public ActionResult BanAccount([FromRoute] int? id)
        {

            var userid = Convert.ToInt32(Request.Cookies["ID"] ?? "-1");

            if (id is null || id == userid)
                return StatusCode((int)HttpStatusCode.NotFound, "Not Valid Data");
            var db = new ProjectDbContext();
            var Item = db.Users.Find(id);
            if (Item is null)
                return StatusCode((int)HttpStatusCode.NotFound, "Not Valid Data");
            else
            {
                Item.IsValid = (byte)(1 - Item.IsValid);
                try
                {
                    db.SaveChanges();
                }
                catch
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                }
            }
            return Ok();

        }

    }
}
