using Microsoft.AspNetCore.Mvc;

namespace FcRoomBooking.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
