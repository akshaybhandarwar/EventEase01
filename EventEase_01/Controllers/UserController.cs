using EventEase_01.Models;
using EventEase_01.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using EventEase_01.ActionAttributes;
using System.Net.Mail;
using System.Net;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging.Console;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace EventEase_01.Controllers
{
    public class UserController : Controller
    {
     

        private readonly EventEase01Context _context;
        private readonly IConfiguration _config;
        private readonly AESEncryption _encryptionService;
        private readonly UserRegistrations _registrationService;
        private readonly JwtToken _jwtToken;
        private readonly EmailService _emailService;
        private readonly OTPService _otpService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ConnectionFactory _factory;
        //private readonly UserManager<IdentityUser> _userManager;


        public UserController(EventEase01Context context, IConfiguration config, AESEncryption encryptionservice, UserRegistrations registrationService,JwtToken jwtToken, EmailService emailService, OTPService otpService, IHttpContextAccessor httpContextAccessor)
        {
            _config = config;
            _context = context;
            _encryptionService = encryptionservice;
            _registrationService = registrationService;
            _jwtToken = jwtToken;
            _emailService = emailService; 
            _otpService = otpService;
            _httpContextAccessor = httpContextAccessor;
            _factory = new ConnectionFactory() { HostName = "localhost" };
            //_userManager = userManager;
        }
        [NoCache]
        public IActionResult Login(string msg)
        {
            string m = msg;
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
        public IActionResult Login(loginModel login, string returnUrl,string msg)
        {
           

            var myUser = _context.Users.FirstOrDefault(x => x.UserEmail == login.Email);
            HttpContext.Session.SetString("UserId", myUser.UserId.ToString());
            ViewData["Id"] = myUser.UserId;
            if (myUser != null)
            {
                string m = msg;
                string key = _config["PasswordKey"];
                string encryptedPassword = _encryptionService.AuthEncrypt(login.Password, myUser.PasswordSalt, key);
                if (encryptedPassword == myUser.PasswordHash)
                {
                    string token = _jwtToken.GenerateToken(myUser);
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
                    else if (myUser.UserRole == "S-admin")
                    {
                        return RedirectToAction("SuperAdminDashboard", "User");
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
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            TempData["UserEmail"] = email;
            string otp = _otpService.GenerateOTP();
            TempData["GenOTP"] = otp;
            string subject = "Forgot Password OTP";
            string body = $"Your OTP for Password Reset is: {otp}";
            await _emailService.SendEmailAsync(email, subject, body);
            return RedirectToAction("VerifyOTP");
        }
        [HttpGet]
        public IActionResult VerifyOTP(string otp)
        {           
            return View(model:otp);
        }
        [HttpPost]
        public IActionResult VerifyOTP(string GeneratedOTP, string EnteredOTP)
        
        {
            GeneratedOTP = TempData["GenOTP"] as string;
            Console.WriteLine("OTP from hidden field: " + GeneratedOTP);
            Console.WriteLine("OTP from input field: " + EnteredOTP);
            if (GeneratedOTP == EnteredOTP)
            {
                return RedirectToAction("ResetPassword");
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid OTP. Please try again.";
                return RedirectToAction("ForgotPassword");
            }
        }
        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ResetPassword(string newPassword)
        {
            
            string userEmail = TempData["UserEmail"] as string;
            var user = _context.Users.FirstOrDefault(e=>e.UserEmail==userEmail);
            Console.WriteLine(newPassword);
            if (user == null)
            {
                return NotFound();
            }
            string ResetPassKey = _config["PasswordKey"];
            string ResetPasswordSalt = null;
            var ResetPasswordHash = _encryptionService.Encrypt(newPassword, out ResetPasswordSalt, ResetPassKey);
            user.PasswordHash = ResetPasswordHash;
            user.PasswordSalt = ResetPasswordSalt;
            _context.SaveChanges();
            TempData["Success"] = "Password Changed Succesfully Please Login";
            return RedirectToAction("Login", "User"); 
        }
        [HttpPost]
        [NoCache]
        public async Task<IActionResult> Registration(RegistrationModel model)
        {
            if (ModelState.IsValid)
            {
                var userRegistrations = new UserRegistrations(_context, _config, _encryptionService);
                bool result = await userRegistrations.RegisterUserAsync(model);
                if (result)
                {
                    TempData["SuccessMessage"] = "Registration successful! Please log in.";
                   

                    return RedirectToAction("Login");
                }
                else
                {
                    TempData["ErrorMessage"] = "Registration failed. Email ID already exists. Please register with a different email ID.";
                }
            }
            return View(model );
        }
        [Authorize(Roles ="user")]
        [NoCache]
        public async Task<IActionResult> Dashboard([FromServices] IDistributedCache cache)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            var cachedData = await cache.GetStringAsync("DashboardData");
            if (cachedData != null)
            {
                var Cachedevents = JsonConvert.DeserializeObject<List<Event>>(cachedData);
                ViewData["Events"] = Cachedevents;
                Console.WriteLine("Data Fetched From Redis ...");
                return View();
            }
            var events = _context.Events.Where(e => e.EventDate > DateTime.Now).ToList();
            await cache.SetStringAsync("DashboardData", JsonConvert.SerializeObject(events), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });
            ViewData["Events"] = events;
            Console.WriteLine("Fetched Events from SQL Server..");
            return View();
        }
        [Authorize(Roles = "admin")]
        [NoCache]
        [HttpGet]
        public async Task<IActionResult> AdminDashboard([FromServices] IDistributedCache cache)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            var cachedData = await cache.GetStringAsync("AdminDashboardData");
            if (cachedData != null)
            {
                var Cachedevents = JsonConvert.DeserializeObject<List<Event>>(cachedData);
                ViewData["Events"] = Cachedevents;
                Console.WriteLine("Data Fetched From Redis ...");
                return View();
            }

            var events = _context.Events.Where(e => e.EventDate > DateTime.Now).ToList();

            await cache.SetStringAsync("AdminDashboardData", JsonConvert.SerializeObject(events), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) 
            });
           
            ViewData["Events"] = events;
            
            Console.WriteLine("Data Fetched From SQL Server ...");
            return View();
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        [NoCache]
        public IActionResult AdminDashboard(EventModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            var events = _context.Events.Where(e => e.EventDate > DateTime.Now).ToList();
            ViewData["Events"] = events;
            return View();
        }

        public async Task<IActionResult> SuperAdminDashboard()
        {
            var events = _context.Events.Where(e => e.EventDate > DateTime.Now).ToList();
            ViewData["Events"] = events;
            Console.WriteLine("Data Fetched from SQL Server");
            return View();
        }
        public ActionResult ManageUser()
        {
            return View();
        }
        public ActionResult PaymentGateway()
        {
            var eventNameObj = TempData["name"];
            Console.WriteLine("*****************" + eventNameObj);
            if (eventNameObj != null)
            {
                var eventName = eventNameObj.ToString(); 
                HttpContext.Session.SetString("EventName", eventName);
            }

            return View();
        }
        //[HttpPost]
        public async Task<ActionResult> UpdateSeatStatus(List<Guid> ticketIds)
        {
            var eventName = TempData["EventName"];
            //TempData["TicketIds"] = ticketIds;
            TempData["name"] = eventName;
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };
            var bookedTickets = new List<Guid>();

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "ticket-booking",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var ticketId = Guid.Parse(Encoding.UTF8.GetString(body));

                    var ticket = _context.Tickets.FirstOrDefault(t => t.TicketId == ticketId);
                    if (ticket != null && ticket.TicketAvailability == 0)
                    {
                        Console.WriteLine($"Ticket {ticketId} already booked.");
                    }
                    else
                    {
                        if (ticket != null)
                        {
                            ticket.TicketAvailability = 0;
                            Console.WriteLine("Message Dequeued From RabbitMQ ");
                           
                            bookedTickets.Add(ticketId);
                            Console.WriteLine($"Booking ticket {ticketId}");
                        }
                    }
                };

                channel.BasicConsume(queue: "ticket-booking",
                                     autoAck: true,
                                     consumer: consumer);
                foreach (var ticketId in ticketIds)
                {
                    await PublishMessage(ticketId, factory);
                    Console.WriteLine("Message Queued to RabbitMQ");
                }
            }
            _context.SaveChanges();
            var userIdString = HttpContext.Session.GetString("UserId");
            if (bookedTickets.Count == ticketIds.Count)
            {
                if (Guid.TryParse(userIdString, out Guid userId))
                {
                    foreach (var ticketId in bookedTickets)
                    {
                        var booking = new Booking
                        {
                            BookingId = Guid.NewGuid(),
                            UserId = userId,
                            TicketId = ticketId,
                            NumberOfBookings = 1,
                            BookingDateTime = DateTime.Now
                        };

                        _context.Bookings.Add(booking);
                        Console.WriteLine("Booking Added ");
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("PaymentGateway");
                //return View("PaymentGateway");
            }
            else
            {
                return View("Error");
            }
        }
        private async Task PublishMessage(Guid ticketId, ConnectionFactory factory)
        {
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "ticket-booking",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(ticketId.ToString());

                channel.BasicPublish(exchange: "",
                                     routingKey: "ticket-booking",
                                     basicProperties: null,
                                     body: body);
            }
        }
        public ActionResult SelectTickets(Guid eventId)
        {
            Console.WriteLine(eventId);
            var eventModel = _context.Events.FirstOrDefault(e => e.EventId == eventId);
            var eventName = _context.Events.FirstOrDefault(e => e.EventId == eventId)?.EventName;
            TempData["EventName"] = eventName;
            var ticket = _context.Tickets.Where(e => e.EventId == eventId).ToList();
            Console.WriteLine(eventModel.NumberOfTickets);
            ViewData["Tickets"] = ticket;
            return View(eventModel);
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