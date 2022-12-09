using Microsoft.AspNetCore.Mvc;

namespace FcRoomBooking.Controllers
{
    public class RoomBookingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult Edit()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Post()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Update()
        {
            return View();
        }
        public IActionResult Delete()
        {
            return View();
        }
    }
}
