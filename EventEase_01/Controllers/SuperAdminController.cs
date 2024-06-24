using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase_01.Models;
using EventEase_01.Services;

namespace EventEase_01.Controllers
{
    
    public class SuperAdminController : Controller
    {
        private readonly EventEase01Context _context;
        private readonly EmailService _emailService;

        public SuperAdminController(EventEase01Context context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

   
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,UserName,UserAge,UserEmail,PasswordHash,PasswordSalt,UserRole")] User user)
        {
            if (ModelState.IsValid)
            {
                user.UserId = Guid.NewGuid();
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("UserId,UserRole")] User editedUser)
        {
            if (id != editedUser.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userToUpdate = await _context.Users.FindAsync(id);

                    if (userToUpdate == null)
                    {
                        return NotFound();
                    }

                    userToUpdate.UserRole = editedUser.UserRole;
                    string? usermail = userToUpdate.UserEmail;
                    _context.Update(userToUpdate);
                    await _context.SaveChangesAsync();
                    if (editedUser.UserRole == "admin")
                    {
                        string subject = "Congratulations on Your Promotion to Admin!";
                        string body = $"Dear {userToUpdate.UserName},\r\n\r\n" +
                                      $"I am pleased to inform you that you have been promoted to the role of {editedUser.UserRole} in our system. " +
                                      $"With this new role, you now have additional privileges, including the ability to add events to our platform. " +
                                      $"As an Administrator, you play a crucial role in shaping our platform and contributing to its success.\r\n\r\n" +
                                      $"We trust you will utilize your new responsibilities with care and integrity. " +
                                      $"Should you have any questions or need assistance, please do not hesitate to reach out to us.\r\n\r\n" +
                                      $"Once again, congratulations on your promotion! .";
                        await _emailService.SendEmailAsync(usermail, subject, body);
                        }
                        
                    
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(editedUser.UserId))
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
            return View(editedUser);
        }


        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool UserExists(Guid id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
