using EventEase_01.Models;
using EventEase_01.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;


namespace EventEase_01.Controllers
{
    public class EventController : Controller
    {
        private readonly EventEase01Context _context;
        public EventController(EventEase01Context context)
        {
            _context = context;
        }
        //[HttpGet]
        public async Task<ActionResult> AdminAddEvents(EventModel model)
        {
            var venue = _context.Venues
                   .Where(e => e.VenueId == model.VenueId)
                   .FirstOrDefault();
        



            Event e1 = new Event
            {
                EventName = model.EventName,
                EventDescription = model.EventDescription,
                EventDate = model.EventDate,
                VenueId = venue.VenueId


            };
            Console.WriteLine("**************************************************************");
            Console.WriteLine(model.VenueId);
            Console.WriteLine(venue.VenueName);
            Console.WriteLine("**************************************************************");
            await _context.Events.AddAsync(e1);
            await _context.SaveChangesAsync();
            TempData["EventAddedMessage"] = "The Event is added. It will be live in a moment. ";
            return View("AdminDashboard");
            
        }
        [HttpGet]
        public  async Task<ActionResult> AdminDashboard() {
            var venues = await _context.Venues.ToListAsync();
           
           
            ViewData["Venues"] = venues;
           
           
            return View("AdminAddEvents");
        }
        [HttpPost]
        public async Task<ActionResult> AdminDashboard(EventModel model )
        {
            //Event e1 = new Event
            //{
            //    EventName = model.EventName,
            //    EventDescription = model.EventDescription,
            //    EventDate = model.EventDate,

            //};
            //await _context.Events.AddAsync(e1);
            //await _context.SaveChangesAsync();
            return View("AdminAddEvents");
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
  
    }
}
