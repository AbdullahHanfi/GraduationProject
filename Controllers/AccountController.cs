using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using GraduationProject.BindingModels;
using GraduationProject.Services;
using Microsoft.AspNetCore.Cors;

namespace GraduationProject.Controllers
{
    [AllowAnonymous]
    [ApiController]
    public class AccountController : ControllerBase
    {

        /// <summary>
        /// For register new user by defult he is student for now
        /// </summary>
        /// <returns>if data correct 201 ; If data is wrong will return 406 and Json file with the wrong fileds ; If unknow problem 500.</returns>
        [HttpPost("register")]
        public async Task<IActionResult>  register([FromBody] RegisterBinding item)
        {
            int ValidRegistertion = await AccountValidate.IsValidRegistertion(item, ModelState);

            if (ValidRegistertion == 0)
            {
                return StatusCode((int)HttpStatusCode.NotAcceptable, ModelState);
            }
            else if (ValidRegistertion == -1)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            else
            {
                return StatusCode((int)HttpStatusCode.Created);
            }
        }



        /// <summary>
        /// For login user in generale
        /// He can login by Name or Email like Codeforces
        /// </summary>
        /// <returns>If data correct 204 ; If data is wrong will return 401 and Json file with the wrong fileds. </returns>
        [HttpPost("login")]
        public async Task<IActionResult> login([FromBody] LoginBinding item)
        {
            if (item == null)
            {
                return StatusCode((int)HttpStatusCode.BadRequest);
            }

            bool Valid = await AccountValidate.IsVaildAccount(item.Email, item.Password, HttpContext);

            if (Valid)
            {
                return StatusCode((int)HttpStatusCode.OK);
            }
            else
            {
                ModelState.AddModelError("Email", "Email or Password is wrong");
                var Errors = ModelState.Values.SelectMany(v => v.Errors);

                return StatusCode((int)HttpStatusCode.Unauthorized, Errors);
            }
        }
        /// <summary>
        /// Just signout 😀
        /// </summary>
        /// <returns>all time 200</returns>
        [HttpGet("signout")]
        public async Task signout()
        {
            if (User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        }
    }
}
