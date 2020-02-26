using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Soaring.AdminWeb.Controllers{
    [Authorize]
    [Route("[controller]")]
    //加了route 下面的方法也得加上 route或者httpget httppost的请求路径
    public class SignController : Controller {
        public SignController(){

        }
        [HttpGet("login")]
        [AllowAnonymous]
        public IActionResult Login(){
            return View();
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string username,string password){
            var returnUrl = HttpContext.Request.Query["ReturnUrl"];
            string roleType = "";
            if(username == "admin"){
                roleType =  "Administrator";
            }
            else if(username == "custom"){
                roleType = "Custom";
            }
            if((username == "admin" && password == "admin") || (username == "custom" && password == "custom")){
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,username),
                    new Claim("Role",roleType)
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties());
                if (!string.IsNullOrWhiteSpace(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return Redirect("/home/");
            }
            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return Redirect("/sign/login");
        }
        [HttpPost("loginout")]
        public async Task<IActionResult> LoginOut(){
            await HttpContext.SignOutAsync();
            return Redirect("/home/"); 
        }
    }
}