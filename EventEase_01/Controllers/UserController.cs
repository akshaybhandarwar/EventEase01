
using EventEase_01.Models;
using EventEase_01.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace EventEase_01.Controllers
{
    public class UserController : Controller
    {
        private readonly EventEase01Context _context;
        private readonly IConfiguration _config;
        private readonly AESEncryption _encryptionService;

        public UserController(EventEase01Context context, IConfiguration config, AESEncryption encryptionservice)
        {
            _config = config;
            _context = context;
            _encryptionService = encryptionservice;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(loginModel login)
        {
            var myUser = _context.Users.Where(x => x.UserEmail == login.Email).FirstOrDefault();
            if (myUser != null)
            {
                HttpContext.Session.SetString("UserSession", myUser.UserEmail);
                string iv;
                string encryptedPassword = _encryptionService.AuthEncrypt(login.Password, myUser.PasswordSalt, myUser.PasswordHash);

                if (encryptedPassword == myUser.PasswordHash)
                {
                    HttpContext.Session.SetString("UserSession", myUser.UserEmail);
                    return RedirectToAction("Dashboard");
                }
                return RedirectToAction("Dashboard");
            }
            else
            {
                ViewBag.Message = "Login Failed";
            }
            return View();
        }
        public IActionResult Registration()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Registration(RegistrationModel model)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model isnt Valid");
            }
            else
            {
                string PassKey = _config["PasswordKey"];
                string PasswordSalt = null;
                var PasswordHash = _encryptionService.Encrypt(model.Password, out PasswordSalt, PassKey);
                User u1 = new User
                {
                    UserName = model.UserName,
                    UserEmail = model.Email,
                    PasswordHash = PasswordHash,
                    PasswordSalt = PasswordSalt,
                };
                await _context.Users.AddAsync(u1);
                await _context.SaveChangesAsync();
                ViewBag.SuccessMessage = "Registration successful! Please log in.";
                return RedirectToAction("Login");
            }
            return View(model);
        }
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("UserSession") != null)
            {
                ViewBag.MySession = HttpContext.Session.GetString("UserSession").ToString();
            }
            else
            {
                return RedirectToAction("Login");
            }
            return View();
        }
      
    }
}