
using EventEase_01.Models;
using EventEase_01.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using EventEase_01.ActionAttributes;
using System.Net.Mail;
using System.Net;


namespace EventEase_01.Controllers
{
    public class UserController : Controller
    {
        private readonly EventEase01Context _context;
        private readonly IConfiguration _config;
        private readonly AESEncryption _encryptionService;
        private readonly UserRegistrations _registrationService;
        private readonly JwtToken _jwtToken;
        
        public UserController(EventEase01Context context, IConfiguration config, AESEncryption encryptionservice, UserRegistrations registrationService,JwtToken jwtToken)
        {
            _config = config;
            _context = context;
            _encryptionService = encryptionservice;
            _registrationService = registrationService;
            _jwtToken = jwtToken;
          
        }
        [NoCache]
        public IActionResult Login()
        {
            return View();
        }


    [HttpGet]
        [NoCache]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("JWTToken");
            return RedirectToAction("Login");
        }
        [HttpPost]
        [NoCache]
        public IActionResult Login(loginModel login, string returnUrl)
        {
            var myUser = _context.Users.FirstOrDefault(x => x.UserEmail == login.Email);

            if (myUser != null)
            {
                string key = _config["PasswordKey"];
                string encryptedPassword = _encryptionService.AuthEncrypt(login.Password, myUser.PasswordSalt, key);

                if (encryptedPassword == myUser.PasswordHash)
                {
                    string token = _jwtToken.GenerateToken(myUser);
                    Console.WriteLine(token);
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict
                    };
                    HttpContext.Response.Cookies.Append("JWTToken", token, cookieOptions);

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    if (myUser.UserRole == "admin")
                    {
                        return RedirectToAction("AdminDashboard", "User");
                    }
                    else
                    {
                        return RedirectToAction("Dashboard", "User");
                    }
                }
            }
            ModelState.AddModelError(string.Empty, "Invalid email or password. Please try again.");
            return RedirectToAction("Login", "User", new { returnUrl });
        }
        [NoCache]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        [NoCache]
        public async Task<IActionResult> Registration(RegistrationModel model)
        {
            if (ModelState.IsValid)
            {

                Console.WriteLine("Entered ModelState is Valid");
                var userRegistrations = new UserRegistrations(_context, _config, _encryptionService);
                bool result = await userRegistrations.RegisterUserAsync(model);
                Console.WriteLine("User registration is done ");
                if (result)
                {

                    TempData["SuccessMessage"] = "Registration successful! Please log in.";
                    return RedirectToAction("Login");
                }

                else
                {
                    Console.WriteLine("Entered Else block Same registration Found ...");
                    TempData["ErrorMessage"] = "Registration failed. Email ID already exists. Please register with a different email ID.";

                }
            }
            return View(model);
        }
        [Authorize]
        [NoCache]
        public IActionResult Dashboard()
        {
            //if (HttpContext.Session.GetString("UserSession") != null)
            //{
            //    ViewBag.MySession = HttpContext.Session.GetString("UserSession").ToString();
            //}
            //else
            //{
            //    return RedirectToAction("Login");
            //}
            var events = _context.Events.ToList();
            ViewData["Events"] = events;
            return View();
        }
  
        [Authorize(Roles ="admin")]
        [NoCache]
        [HttpGet]
        public IActionResult AdminDashboard()
        {
            var events = _context.Events.ToList();
            ViewData["Events"] = events;
            return View();
        }


        [HttpPost]
        [NoCache]
        public IActionResult AdminDashboard(EventModel model)
        {
            var events = _context.Events.ToList();
            return View();
        }
        public ActionResult PaymentGateway()
        {
            return View();
        }
        public ActionResult SelectTickets()
        {
            var selectedEvent = ViewData["Events"] as EventEase_01.Models.Event;
            if (selectedEvent != null)
            {
                var countOfTickets = _context.Events.Where(e => e.EventId == selectedEvent.EventId)
                                                    .Select(e => e.NumberOfTickets)
                                                    .FirstOrDefault();
            
            ViewData["CountOfTickets"] = countOfTickets;
        }
            return View();
        }

       
        public ActionResult EventDescription(Guid eventId)
        {

        
            var eventDetails = (from e in _context.Events
                                join v in _context.Venues on e.VenueId equals v.VenueId
                                where e.EventId == eventId
                                select new { Event = e, VenueName = v.VenueName, VenueAddress = v.VenueAddress })
                   .FirstOrDefault();
            if (eventDetails != null)
            {
                ViewData["Events"] = eventDetails.Event;
                ViewData["VenueName"] = eventDetails.VenueName;
                ViewData["VenueAddress"] = eventDetails.VenueAddress;
            }

            return View();
        }
    }
}

