using EventEase_01.Models;
using EventEase_01.Services;
using Microsoft.AspNetCore.Mvc;

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
        public ActionResult AdminAddEvents() {

            //return View("AdminAddEvents");
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AdminAddEvents(EventModel model) {
            Event e1 = new Event
            {
                EventName = model.EventName,
                EventDescription = model.EventDescription,
                EventDate = model.EventDate
            };
            await _context.Events.AddAsync(e1);
            await _context.SaveChangesAsync();
            TempData["EventAddedMessage"] = "The Event is added. It will be live in a moment. ";
            return View("AdminDashboard");


        }
    }
}
