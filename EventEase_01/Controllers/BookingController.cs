using EventEase_01.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace EventEase_01.Controllers
{
    public class BookingController : Controller
    {
        private readonly EventEase01Context _context;
        public BookingController(EventEase01Context context)
        {
            _context = context;
        }
        public ActionResult BookingAction()
        {
            var eventName = HttpContext.Session.GetString("EventName");
            return RedirectToAction("TicketConfirmation", "Ticket", new { eventName = eventName });
        }
        public ActionResult Check()
        {
            return View();
        }
        public ActionResult ViewBookings()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (Guid.TryParse(userIdString, out Guid userId))
            {
                var res = (from booking in _context.Bookings
                           where booking.UserId == userId
                           select booking.TicketId).ToList();

                var eventIds = (from ticket in _context.Tickets
                                where res.Contains(ticket.TicketId)
                                select ticket.EventId)
               .Distinct()
               .ToList();
               
                var eventDetails = (from e in _context.Events
                                    join eventId in eventIds on e.EventId equals eventId
                                    select new Event
                                    {
                                        EventName=e.EventName,
                                        EventDescription=e.EventDescription,
                                        EventCity=e.EventCity
                                    }).ToList();

               
                ViewData["data"] = eventDetails;
                TempData["eventCount"] = eventIds.Count;
                TempData["totalNumberOfBookings"] = res.Count;
            }
            return View();
        }
    }
}
