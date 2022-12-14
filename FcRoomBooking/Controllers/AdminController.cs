using FcRoomBooking.Areas.Identity.Data;
using FcRoomBooking.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FcRoomBooking.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public AdminController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        public JsonResult GetData()
        {
            var eventList = new List<EventViewModel>();
            var events = dbContext.RoomBookings.Where(x=>x.BookingStatus == "Active" || x.BookingStatus == "Ongoing").Include(x=>x.ApplicationUser).Include(x=>x.Room).ToList();
            foreach (var item in events)
            {
                var startTime = item.BookingFrom.ToString("hh:mm");
                var endTime = item.BookingTo.ToString("hh:mm");
                eventList.Add(new EventViewModel
                {
                    Id = item.Id,
                    title = item.Subject,
                    start = item.BookingFrom.ToString("yyyy-MM-dd").Trim()+"T"+startTime,
                    end = item.BookingTo.ToString("yyyy-MM-dd").Trim()+"T"+endTime,
                    color = (item.BookingStatus == "Active"? "green" : (item.BookingStatus == "Ongoing")? "blue" : "red"),
                    allDay = false,
                    detail = item.Detail,
                    bookby = item.ApplicationUser.UserName,
                    room = item.Room.RoomName,
                }) ;
            }
            return Json(eventList);
        }
        
    }
}
