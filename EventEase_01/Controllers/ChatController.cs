using Microsoft.AspNetCore.Mvc;

namespace EventEase_01.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
