using GraduationProject.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;
using GraduationProject.BindingModels;

namespace GraduationProject.Services
{

    static public class AccountValidate
    {
        /// <summary>
        /// insert Student into DataBase
        /// </summary>
        /// <returns>return one if Data inserted correctly or zero if Data isn't Valid or -1 if unknow Error</returns>
        static public async Task<int> IsValidRegistertion(RegisterBinding item, ModelStateDictionary ModelState)
        {
            using (var db = new ProjectDbContext())
            {
                bool ValidEmail =await AccountValidate.ValidEmail(item.Email);
                bool ValidName =await AccountValidate.ValidName(item.Name);

                if (!ValidEmail)
                {
                    ModelState.AddModelError("Email", "Email Not Valid");
                }
                if (!ValidName)
                {
                    ModelState.AddModelError("Name", "Name Not Valid");
                }
                if (!ModelState.IsValid)
                {
                    return 0;
                }
                else
                {
                    await db.Users.AddAsync((User)item);
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
            }
        }

        /// <summary>
        /// Make Cookie for user 
        /// </summary>
        /// <returns>return one if Account Data is Valid or Zero if Data isn't Valid</returns>
        static public async Task<bool> IsVaildAccount(string UserName, string password, HttpContext HttpContext)
        {
            using (var db = new ProjectDbContext())
            {
                var User = db.Users
                .FirstOrDefault(M => (M.Email == UserName || M.Name == UserName)
                && M.Password == password);

                if (User != null && User.IsValid == 1 && User.CheatTimes < 3)
                {
                    var claims = new List<Claim>{
                                new Claim(type:"ID", User.Id.ToString()),
                                new Claim(type: ClaimTypes.Name, User.UserName),
                                new Claim(type: ClaimTypes.Role, db.Roles.Find(User.RId).Name)
                    };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(
                    scheme: CookieAuthenticationDefaults.AuthenticationScheme,
                    principal: new ClaimsPrincipal(identity),
                    properties: new AuthenticationProperties
                    {
                        IsPersistent = true,
                        AllowRefresh = true
                    });

                    return true;
                }
                return false;
            }
        }

        static private async Task<bool> ValidEmail(string Email)
        {
            using (var db = new ProjectDbContext())
            {
                return !db.Users
                    .Where(M => M.Email == Email)
                    .Any();
            }
        }

        static private async Task<bool> ValidName(string Name)
        {
            using (var db = new ProjectDbContext())
            {
                return !db.Users
                    .Where(M => M.Email == Name)
                    .Any();
            }
        }
    }
}