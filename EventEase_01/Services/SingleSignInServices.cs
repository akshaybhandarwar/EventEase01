using EventEase_01.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;

namespace EventEase_01.Services
{
    public class SingleSignInServices
    {
        private readonly EventEase01Context _context;
        private readonly IConfiguration _config;
        private readonly AESEncryption _encryptionService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JwtToken _jwtToken;
        public SingleSignInServices(EventEase01Context context,
                                    IConfiguration config,
                                    AESEncryption encryptionService,
                                    IHttpContextAccessor httpContextAccessor,
                                    JwtToken jwtToken)
        {
            _context = context;
            _config = config;
            _encryptionService = encryptionService;
            _httpContextAccessor = httpContextAccessor;
            _jwtToken = jwtToken;
        }
        //public IActionResult Login(loginModel login, bool isSinglesignIn)
        //{
        //    var myUser = _context.Users.FirstOrDefault(x => x.UserEmail == login.Email);
        //    if (myUser != null && isSinglesignIn==true)
        //    {
        //        _httpContextAccessor.HttpContext.Session.SetString("UserId", myUser.UserId.ToString());
        //        string token = _jwtToken.GenerateToken(myUser);
        //        var cookieOptions = new CookieOptions
        //        {
        //            HttpOnly = true,
        //            Secure = true,
        //            SameSite = SameSiteMode.Strict
        //        };
        //        _httpContextAccessor.HttpContext.Response.Cookies.Append("JWTToken", token, cookieOptions);


        //        if (myUser.UserRole == "admin")
        //        {
        //            return new RedirectToActionResult("AdminDashboard", "User", null);
        //        }
        //        else if (myUser.UserRole == "S-admin")
        //        {
        //            return new RedirectToActionResult("SuperAdminDashboard", "User", null);
        //        }
        //        else
        //        {
        //            return new RedirectToActionResult("Dashboard", "User", null);
        //        }
        //    }
        //    return new RedirectToActionResult("Login", "User", null);
        //}

        public bool Login(loginModel login, bool isSinglesignIn)
        {
            var myUser = _context.Users.FirstOrDefault(x => x.UserEmail == login.Email);
            if (myUser != null && isSinglesignIn)
            {
                _httpContextAccessor.HttpContext.Session.SetString("UserId", myUser.UserId.ToString());
                string token = _jwtToken.GenerateToken(myUser);
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                };
                _httpContextAccessor.HttpContext.Response.Cookies.Append("JWTToken", token, cookieOptions);

                return true;
            }
            return false;
        }

    }
}


