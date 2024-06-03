using EventEase_01.Models;
using Microsoft.AspNetCore.Mvc;
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
            //return View("~/Views/Ticket/TicketConfirmation.cshtml");           
        }
    }
}

