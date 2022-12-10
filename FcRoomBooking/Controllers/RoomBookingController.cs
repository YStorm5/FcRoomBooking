using FcRoomBooking.Areas.Identity.Data;
using FcRoomBooking.Models.Domain;
using FcRoomBooking.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FcRoomBooking.Controllers
{
    public class RoomBookingController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public RoomBookingController(ApplicationDbContext dbContext,UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            ViewBag.TimeList = TimePicker();
            ViewBag.roomList = RoomList();
            return View();
        }
        public IActionResult Participant()
        {
            return View();
        }
        public IActionResult Edit()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Post(RoomBookingViewModel Request)
        {
            DateTime dateFrom = DateTime.Parse(Request.BookingFrom +" "+ Request.BookingFromTime);
            DateTime dateTo = DateTime.Parse(Request.BookingTo + " " + Request.BookingToTime);
            var user = await userManager.GetUserAsync(User);
            await dbContext.RoomBookings.AddAsync(new RoomBooking()
            {
                RoomId = Request.RoomId,
                UserId = user.Id,
                Subject = Request.Subject,
                Detail= Request.Detail,
                BookingFrom= dateFrom,
                BookingTo= dateTo,
                BookingStatus = "Active"
            });
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

        public List<SelectListItem> TimePicker()
        {
            List<SelectListItem> List = new List<SelectListItem>();
            for (int i = 0; i < 24; i++)
            {
                for(float y = 0; y < 31; y += 30)
                {
                    List.Add(new SelectListItem
                    {
                        Text = $"{i}:{(y!=0?y:"00")}",
                        Value = $"{i}:{y}"
                    });
                }
            }
            return List;
        }
        public List<SelectListItem> RoomList()
        {
            List<SelectListItem> List = new List<SelectListItem>();
            var roomList = dbContext.Rooms.ToList();
            foreach (var item in roomList)
            {
                List.Add(new SelectListItem
                {
                    Text = item.RoomName,
                    Value = item.Id.ToString()
                });
            }
            return List;
        }
    }
}
