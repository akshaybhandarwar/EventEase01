﻿using EventEase_01.Models;
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
        private readonly IWebHostEnvironment _webHostEnvironment;
        public EventController(EventEase01Context context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<string> UploadImage(IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                try
                {
                    //var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "EventImages");
                    var uploadsFolder = "C:\\Users\\Coditas-Admin\\source\\repos\\EventEase_01\\EventEase_01\\wwwroot\\EventImages";


                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;

                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    Directory.CreateDirectory(uploadsFolder);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }
                    return "/EventImages/" + uniqueFileName;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    // Return a default image path in case of error
                    return "/EventImages/default-Event.png";
                }
            }
            else
            {
                // Return the default image path if no file is uploaded
                return "/EventImages/default-Event.png";
            }
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
        [HttpPost]//-Hits after a new event is created ....
        public async Task<ActionResult> AdminAddEvents(EventModel model)
        {
            var venue = _context.Venues
                   .Where(e => e.VenueId == model.VenueId)
                   .FirstOrDefault();
            var category= _context.Categories.
                Where(e=>e.CategoryId==model.CategoryId).
                FirstOrDefault();
            Console.WriteLine("****************");
            Console.WriteLine(model.EventImage.FileName);
           
            string imagePath = await UploadImage(model.EventImage);

            Event e1 = new Event
            {
                EventName = model.EventName,
                EventDescription = model.EventDescription,
                EventDate = model.EventDate,
                VenueId = venue.VenueId,
                CategoryId=category.CategoryId,
                EventImageFileName= imagePath,
                NumberOfTickets=model.NumberOfTickets

            };
            Console.WriteLine("***********************");
            Console.WriteLine(e1.EventImageFileName);
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
            var events =_context.Events.Where(e=>e.EventDate>DateTime.Now).ToList();
            TempData["EventAddedMessage"] = "The Event is added. It will be live in a moment. ";
            ViewData["Events"] = events;
            return View();
        }
        //public ActionResult ShowEvents()
        //{
        //    //var events = _context.Events.ToList();
        //    var events = _context.Events.Where(e => e.EventDate > DateTime.Now).ToList();
        //    return View(events);
        //return RedirectToAction("AdminDashboard");
        //}
        public ActionResult ShowEvents()
        {
          
            var events = _context.Events.ToList();
            ViewData["Events"] = events;
            return View();
          
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
            ViewData["Events"] = events;
            return View("ShowEvents");
        }
        public ActionResult Dance()
        {
            var categoryId = new Guid("4DA741ED-C888-4D66-B668-29D098C66DF4");
            var events = _context.Events.Where(e => e.CategoryId == categoryId).ToList();
            ViewData["Events"] = events;
            return View("ShowEvents");
        }
        public ActionResult Music()
        {
            var categoryId = new Guid("C130AA81-62DD-40A6-8413-A22D2E72B365");
            var events = _context.Events.Where(e => e.CategoryId == categoryId).ToList();
            ViewData["Events"] = events;
            foreach(var item in events)
            {
                Console.WriteLine(item.EventName);
            }
            return View("ShowEvents");
        }
        public ActionResult Celebration()
        {
            var categoryId = new Guid("E5F54FF5-B244-4B48-B8EC-C2007216A533");
            var events = _context.Events.Where(e => e.CategoryId == categoryId).ToList();
            ViewData["Events"] = events;
            return View("ShowEvents");
        }
        public ActionResult Meditation()
        {
            var categoryId = new Guid("A49BF1E0-6DDC-4282-92D2-6642BDD65469");
            var events = _context.Events.Where(e => e.CategoryId == categoryId).ToList();
            ViewData["Events"] = events;
            return View("ShowEvents");
        }
    
        public ActionResult EventDescription(Guid eventId)
        {
           
            var eventDetails = (from e in _context.Events
                                join v in _context.Venues on e.VenueId equals v.VenueId
                                where e.EventId == eventId
                                select new { Event = e, VenueName = v.VenueName , VenueAddress=v.VenueAddress})
                   .FirstOrDefault();
            Console.WriteLine("***********************");
            Console.WriteLine(eventId);
           
            Console.WriteLine("***********************");
        

            if (eventDetails != null)
            {
                Console.WriteLine(eventDetails.Event.EventImageFileName);
            }
            if (eventDetails != null)
            {
                ViewData["Events"] = eventDetails.Event;
                ViewData["VenueName"] = eventDetails.VenueName;
                ViewData["VenueAddress"] = eventDetails.VenueAddress;
              
            }
           
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

    }
}

