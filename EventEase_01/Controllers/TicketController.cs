using EventEase_01.Models;
using EventEase_01.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using System.Text;
using System;
using System.Text.Json;

namespace EventEase_01.Controllers
{
    public class TicketController : Controller
    {
        private readonly EventEase01Context _context;
        public TicketController(EventEase01Context context)
        {
       
            _context = context;
         
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult TicketConfirmation(string eventName)
        {
           
            var userIdString = HttpContext.Session.GetString("UserId");
            Console.WriteLine("Getting user Id ");
            Console.WriteLine(userIdString);
            TempData["EventName"] = eventName;
            var eventData = _context.Events
                      .Where(e => e.EventName == eventName)
                      .Select(e => new { e.EventCity, e.EventDate })
                      .FirstOrDefault();
            ViewBag.EventCity = eventData?.EventCity;
            ViewBag.EventDate = eventData?.EventDate;

            return View(); 
        }

    }
}
