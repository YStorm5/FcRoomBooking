using FcRoomBooking.Areas.Identity.Data;
using FcRoomBooking.Models.Domain;
using FcRoomBooking.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FcRoomBooking.Controllers
{
    public class RoomBookingController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public RoomBookingController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        public IActionResult Index()
        {
            var list = dbContext.RoomBookings.Include(x => x.Room).Include(x => x.ApplicationUser).Where(x=>x.BookingStatus =="Active" || x.BookingStatus == "Ongoing").ToList();
            return View(list);
        }
        [HttpGet("RoomBooking/{id}")]
        public IActionResult Index(int id)
        {
            var list = dbContext.RoomBookings.Include(x => x.Room).Include(x => x.ApplicationUser).Where(x=>x.BookingStatus =="Active" || x.BookingStatus == "Ongoing").Where(x=>x.Id == id).ToList();
            return View(list);
        }
        public IActionResult Participant(int id)
        {
            var list = dbContext.Participants.Include(x=>x.ApplicationUser).ToList().FindAll(x => x.RoomBookingId == id);
            var newlist = new List<ParticipantViewModel>();
            if (list.Any())
            {
                foreach (var item in list)
                {
                    newlist.Add(new ParticipantViewModel
                    {
                        RoomBookingId = id,
                        Username = item.ApplicationUser.UserName,
                        Email = item.ApplicationUser.Email,

                    });
                }
                ViewBag.BookingId = id;
            }
            return View(newlist);

        }
    }
}
