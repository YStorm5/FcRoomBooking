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
        private readonly RoleManager<ApplicationRole> roleManager;

        public RoomBookingController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
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
        [HttpPost]
        public async Task<IActionResult> CreateRoom(Room room, List<IFormFile> image)
        {
            if (!ModelState.IsValid)
            {
                foreach (var item in image)
                {
                    if (item.Length > 0)
                    {
                        using (var stream = new MemoryStream())
                        {
                            await item.CopyToAsync(stream);
                            room.RoomImage = stream.ToArray();
                        }
                    }
                }

                await _applicationDbContext.Rooms.AddAsync(room);
                await _applicationDbContext.SaveChangesAsync();

                return RedirectToAction("CreateRoom");
            }
            return View(room);
        }
        public IActionResult ListRoom()
        {
            var Room = _applicationDbContext.Rooms.ToList();
            return View(Room);
        }
        [HttpGet]
        public async Task<IActionResult> UpdateRoom(int id)
        {
            var room = await _applicationDbContext.Rooms.FirstOrDefaultAsync(x => x.Id == id);
            return View(room);

        }
        [HttpPost]
        public async Task<IActionResult> UpdateRoom(Room model)
        {
            var room = await _applicationDbContext.Rooms.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (model.image != null)
            {
                if (model.image.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        await model.image.CopyToAsync(stream);
                        model.RoomImage = stream.ToArray();
                    }
                }
            }
            else
            {
                model.RoomImage = room?.RoomImage;
            }
            room.RoomImage = model.RoomImage;
            room.RoomName = model.RoomName;
            room.RoomDescription = model.RoomDescription;
            _applicationDbContext.Rooms.Update(room);
            await _applicationDbContext.SaveChangesAsync();
            return RedirectToAction("ListRoom");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteRoom(Room model)
        {
            var room = await _applicationDbContext.Rooms.FindAsync(model.Id);
            _applicationDbContext.Rooms.Remove(room);
            await _applicationDbContext.SaveChangesAsync();
            return RedirectToAction("ListRoom");
        }
    }
}
