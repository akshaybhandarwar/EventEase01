using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase_01.Models;
using Humanizer.Localisation;
using Microsoft.Extensions.Caching.Distributed;

namespace EventEase_01.Controllers
{
    public class AdminEventsController : Controller
    {
        private readonly EventEase01Context _context;

        public AdminEventsController(EventEase01Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var eventEase01Context = _context.Events
                                                .Include(e => e.Category)
                                                .Include(e => e.Venue);
            return View(await eventEase01Context.ToListAsync());
        }
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var @event = await _context.Events
                .Include(e => e.Category)
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", @event.CategoryId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId", @event.VenueId);
            return View(@event);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("EventId,EventName,EventDescription,EventDate,VenueId,CategoryId,EventImageFileName,NumberOfTickets")] Event @event)
        {
            if (id != @event.EventId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Entry(@event).State = EntityState.Detached;
                    var originalEvent = await _context.Events.FindAsync(id);
           
                    if (originalEvent.NumberOfTickets != @event.NumberOfTickets)
                    {
                        var tickets = _context.Tickets.Where(t => t.EventId == id).ToList();
                      
                        for (int i = tickets.Count; i < @event.NumberOfTickets; i++)
                        {
                            var newTicket = new Ticket
                            {
                                EventId = id,
                                TicketAvailability = 1 
                                                      
                            };
                            _context.Tickets.Add(newTicket);
                        }
                    }
                    originalEvent.EventName = @event.EventName;
                    originalEvent.EventDescription = @event.EventDescription;
                    originalEvent.EventDate = @event.EventDate;
                    originalEvent.VenueId = @event.VenueId;
                    originalEvent.CategoryId = @event.CategoryId;
                    originalEvent.EventImageFileName = @event.EventImageFileName;
                    originalEvent.NumberOfTickets = @event.NumberOfTickets;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.EventId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", @event.CategoryId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId", @event.VenueId);
            return View(@event);
        }


        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.Category)
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (@event == null)
            {
                return NotFound();
            }

            var tickets = await _context.Tickets
                .Where(t => t.EventId == id)
                .ToListAsync();

            if (tickets.Any())
            {
                _context.Tickets.RemoveRange(tickets);
                await _context.SaveChangesAsync();
            }
            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
           
            return View(@event);
            //return RedirectToAction(nameof(Index));
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id, [FromServices] IDistributedCache cache)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {

                _context.Events.Remove(@event);
            }
            await _context.SaveChangesAsync();
             cache.Remove("DashboardData");
             cache.Remove("AdminDashboardData");
            return RedirectToAction(nameof(Index));
        }
        private bool EventExists(Guid id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }
    }
}
