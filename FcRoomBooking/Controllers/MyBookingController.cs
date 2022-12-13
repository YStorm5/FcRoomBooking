using FcRoomBooking.Areas.Identity.Data;
using FcRoomBooking.Models.Domain;
using FcRoomBooking.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FcRoomBooking.Controllers
{
    [Authorize]
    public class MyBookingController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public MyBookingController(ApplicationDbContext dbContext,UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        public IActionResult Index()
        {
            var user = userManager.GetUserId(User);
            var list = dbContext.RoomBookings.Include(x=>x.Room).Include(x=>x.ApplicationUser).Where(x=>x.UserId == user).ToList();
            return View(list);
        }
        public IActionResult Create()
        {
            ViewBag.TimeList = TimePicker();
            ViewBag.roomList = RoomList();
            return View();
        }
        public IActionResult Participant(int id)
        {
            ViewBag.partList = ParticipantList();
            var list = dbContext.Participants.ToList().FindAll(x=>x.RoomBookingId== id);
            if(list.Any())
            {
                var newlist = new List<ParticipantViewModel>();
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
                
                return View(newlist);
            }
            else
            {
                ViewBag.BookingId = id;
                return View();

            }

        }
        [HttpPost]
        public async Task<IActionResult> PostParticipant(ParticipantViewModel Request,int id)
        {
            foreach (var item in Request.ParticipantId)
            {
                var user = await userManager.FindByIdAsync(item);
                var check = dbContext.Participants.Where(x=>x.UserId == item && x.RoomBookingId == id);
                if (check.Any())
                {
                    TempData["Dupe"] = check.First().ApplicationUser.UserName;
                    return RedirectToAction("Participant",new {id=id});
                }
                dbContext.Participants.Add(new Participant
                {
                    UserId= user.Id,
                    RoomBookingId= id,
                });

            }
            dbContext.SaveChanges();
            TempData["isInvited"] = true;
            return RedirectToAction("Participant",new {id=id});
        }
        public IActionResult Edit(int id)
        {
            var room = dbContext.RoomBookings.FirstOrDefault(x=>x.Id == id);
            ViewBag.TimeList = TimePicker();
            ViewBag.roomList = RoomList();
            var tang = room.BookingFrom.ToShortTimeString();
            var newRoom = new RoomBookingViewModel()
            {
                Id = room.Id,
                RoomId = room.RoomId,
                RoomName = room.Room.RoomName,
                Subject = room.Subject,
                Detail = room.Detail,
                UserId= room.UserId,
                BookingFrom = room.BookingFrom.ToString("yyyy-MM-dd"),
                BookingFromTime = room.BookingFrom.ToShortTimeString().Substring(0,5).Trim(),
                BookingTo = room.BookingTo.ToString("yyyy-MM-dd"),
                BookingToTime = room.BookingTo.ToShortTimeString().Substring(0,5).Trim()
            };
            return View(newRoom);
        }
        [HttpPost]
        public async Task<IActionResult> Post(RoomBookingViewModel Request)
        {
            DateTime dateFrom = DateTime.Parse(Request.BookingFrom +" "+ Request.BookingFromTime);
            DateTime dateTo = DateTime.Parse(Request.BookingTo + " " + Request.BookingToTime);
            var user = await userManager.GetUserAsync(User);
            await dbContext.RoomBookings.AddAsync(new RoomBooking()
            {
                Id = Request.Id,
                RoomId = Request.RoomId,
                UserId = user.Id,
                Subject = Request.Subject,
                Detail= Request.Detail,
                BookingFrom= dateFrom,
                BookingTo= dateTo,
                BookingStatus = "Active"
            });
            dbContext.SaveChanges();
            TempData["isCreated"] = true;
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Remove(string id,int url)
        {
            var user = await dbContext.Participants.FirstOrDefaultAsync(x=>x.ApplicationUser.UserName== id);
            dbContext.Participants.Remove(user);
            dbContext.SaveChanges();
            TempData["isRemoved"] = true;
            return RedirectToAction("Participant",new {id=url});
        }
        [HttpPost]
        public IActionResult Update(RoomBookingViewModel Request,int id,string? isCancel)
        {
            var booking = dbContext.RoomBookings.FirstOrDefault(x=>x.Id == Request.Id);
            if(isCancel == "true")
            {
                booking.BookingStatus = "Cancelled";
            }
            else
            {
                if (booking != null)
                {
                    booking.RoomId = Request.RoomId;
                    booking.Subject = Request.Subject;
                    booking.Detail = Request.Detail;
                    booking.BookingFrom = DateTime.Parse(Request.BookingFrom + " " + Request.BookingFromTime);
                    booking.BookingTo = DateTime.Parse(Request.BookingTo + " " + Request.BookingToTime);
                }
            }
            dbContext.SaveChanges();
            TempData["isUpdated"] = true;
            return RedirectToAction("index");
        }
        public IActionResult Delete(int id)
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
                        Value = $"{i}:{(y != 0 ? y : "00")}"
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
        public List<SelectListItem> ParticipantList()
        {
            List<SelectListItem> List = new List<SelectListItem>();
            var list = userManager.Users.ToList().Where(x => x.UserName != User.Identity.Name);
            foreach (var item in list)
            {
                List.Add(new SelectListItem
                {
                    Text=item.UserName,Value = item.Id.ToString()
                });
            }
            return List;
        }
    }
}
