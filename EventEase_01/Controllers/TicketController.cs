using Microsoft.AspNetCore.Mvc;

namespace EventEase_01.Controllers
{
    public class TicketController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult TicketConfirmation(List<Guid> ticketIds)
        {
            foreach (var ticketId in ticketIds)
            {
                Console.WriteLine("**********************");
                Console.WriteLine(ticketId);
            }


            return View();
        }
    }
}

