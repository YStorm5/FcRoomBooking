using FcRoomBooking.Areas.Identity.Data;
using FcRoomBooking.Models.Domain;
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
        public async Task<IActionResult> Index()
        {
            var checkActive = await dbContext.RoomBookings.Where(x=>x.BookingTo<DateTime.Now && x.BookingStatus == "Active").ToListAsync();
            var checkOngoing = await dbContext.RoomBookings.FirstOrDefaultAsync(x => x.BookingFrom <= DateTime.Now && x.BookingTo >= DateTime.Now);
            if (checkActive.Any())
            {
                foreach (var item in checkActive)
                {
                    item.BookingStatus = "Finish";
                    await dbContext.SaveChangesAsync();
                }
                return View();
            }
            if (checkOngoing != null)
            {
                checkOngoing.BookingStatus = "Ongoing";
                await dbContext.SaveChangesAsync();
            }
            return View();
        }
        public JsonResult GetData()
        {
            var eventList = new List<EventViewModel>();
            var events = dbContext.RoomBookings.Where(x=>x.BookingStatus == "Active" || x.BookingStatus == "Ongoing").Include(x=>x.ApplicationUser).Include(x=>x.Room).ToList();
            foreach (var item in events)
            {
                var startTime = item.BookingFrom.ToString("HH:mm");
                var endTime = item.BookingTo.ToString("HH:mm");
                eventList.Add(new EventViewModel
                {
                    Id = item.Id,
                    title = item.Subject,
                    start = item.BookingFrom.ToString("yyyy-MM-dd").Trim()+"T"+startTime,
                    end = item.BookingTo.ToString("yyyy-MM-dd").Trim()+"T"+endTime,
                    color = (item.BookingStatus == "Active"? "#3b82f5" : (item.BookingStatus == "Ongoing")? "green" : "red"),
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
