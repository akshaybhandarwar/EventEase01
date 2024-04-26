using EventEase_01.Models;
using EventEase_01.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EventEase_01.ViewModels;
using System;

namespace EventEase_01.Controllers
{
    public class EventController : Controller
    {
        private readonly EventEase01Context _context;
        public EventController(EventEase01Context context)
        {
            _context = context;
        }
        [HttpGet]//-> Hits When Clicked on Create a new Event - lands on Admin Add Event view 
        public ActionResult AdminAddEvents()
        {
            var venues = _context.Venues.ToList();
            var categories = _context.Categories.ToList();
            var events = _context.Events.ToList();

            ViewData["Venues"] = venues;
            ViewData["Categories"] = categories;
            ViewData["Events"] = events;
            return View();
            
        }
        [HttpPost]//-Hits after a new event is created 
        public async Task<ActionResult> AdminAddEvents(EventModel model)
        {
            var venue = _context.Venues
                   .Where(e => e.VenueId == model.VenueId)
                   .FirstOrDefault();
            var category= _context.Categories.
                Where(e=>e.CategoryId==model.CategoryId).
                FirstOrDefault();
            Event e1 = new Event
            {
                EventName = model.EventName,
                EventDescription = model.EventDescription,
                EventDate = model.EventDate,
                VenueId = venue.VenueId,
                CategoryId=category.CategoryId

            };
            await _context.Events.AddAsync(e1);
            await _context.SaveChangesAsync();
            TempData["EventAddedMessage"] = "The Event is added. It will be live in a moment. ";
            var events = _context.Events.ToList();
            ViewData["Events"] = events;
            return View("AdminDashboard");

        }

        [HttpGet]
        public async Task<ActionResult> AdminDashboard( )
        {
          
            //var venue = _context.Venues
            //       .Where(e => e.VenueId == model.VenueId)
            //       .FirstOrDefault();
            //var category = _context.Categories.
            //    Where(e => e.CategoryId == model.CategoryId).
            //    FirstOrDefault();
            //Event e1 = new Event
            //{
            //    EventName = model.EventName,
            //    EventDescription = model.EventDescription,
            //    EventDate = model.EventDate,
            //    VenueId = venue.VenueId,
            //    CategoryId = category.CategoryId

            //};
            await _context.Events.AddAsync(e1);
            await _context.SaveChangesAsync();
            var events =_context.Events.ToList();
            TempData["EventAddedMessage"] = "The Event is added. It will be live in a moment. ";
            ViewData["Events"] = events;
            return View();
        }
  

        public ActionResult ShowEvents()
        {
            var events = _context.Events.ToList();
            return View(events);
        }
   
        public ActionResult GetEvent()
        {
            var events = _context.Events.ToList();
            return View(events);
        }
        public ActionResult Sports()
        {
            var categoryId = new Guid("F965E862-DBE5-4661-9A80-6CA53DC7247E");
            var events = _context.Events.Where(e => e.CategoryId == categoryId).ToList();
            return View("ShowEvents", events);
        }
        public ActionResult Dance()
        {
            var categoryId = new Guid("4DA741ED-C888-4D66-B668-29D098C66DF4");
            var events = _context.Events.Where(e => e.CategoryId == categoryId).ToList();
            return View("ShowEvents", events);
        }
        public ActionResult Music()
        {
            var categoryId = new Guid("C130AA81-62DD-40A6-8413-A22D2E72B365");
            var events = _context.Events.Where(e => e.CategoryId == categoryId).ToList();
            return View("ShowEvents", events);
        }
        public ActionResult Celebration()
        {
            var categoryId = new Guid("E5F54FF5-B244-4B48-B8EC-C2007216A533");
            var events = _context.Events.Where(e => e.CategoryId == categoryId).ToList();
            return View("ShowEvents", events);
        }
        public ActionResult Meditation()
        {
            var categoryId = new Guid("A49BF1E0-6DDC-4282-92D2-6642BDD65469");
            var events = _context.Events.Where(e => e.CategoryId == categoryId).ToList();
            return View("ShowEvents", events);
        }


    }
}
