using EventEase_01.Models;
using EventEase_01.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using System.Text;
using System;
using System.Text.Json;
using Microsoft.Extensions.Logging;

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
            int? numberOfTickets = HttpContext.Session.GetInt32("BookedTicketsCount");
            Console.WriteLine("Number of Tickets"+numberOfTickets);
            TempData["NumberOfTickets"]=numberOfTickets;
            TempData["EventName"] = eventName;
            var eventid = _context.Events
                                   .Where(e => e.EventName == eventName)
                                   .Select(e => e.EventId)
                                   .FirstOrDefault();
            Console.WriteLine("Eventid:" + eventid.ToString());
            var ticket = _context.Tickets
                .Where(t => t.EventId == eventid)
                .FirstOrDefault();
            if (ticket != null)
            {
                TempData["TicketPrice"] =numberOfTickets* ticket.TicketPrice;
            }
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
