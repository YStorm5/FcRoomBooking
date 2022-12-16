using FcRoomBooking.Areas.Identity.Data;
using FcRoomBooking.Models.Domain;
using FcRoomBooking.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FcRoomBooking.Controllers
{
    public class RoomBookingController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public RoomBookingController(ApplicationDbContext Application)
        {
            _applicationDbContext = Application;
        }
        
        public async Task<IActionResult> ListMyMeeting(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var listschedule = await _applicationDbContext.RoomBookings.Include(x=>x.Room).Include(x => x.RoomStatus).OrderByDescending(x=>x.Id).Where(x => x.UserId == userId).ToListAsync();
            //ViewBag.number = listschedule.Count;
            return View(listschedule);
        }
        [HttpGet]
        public async Task<IActionResult> CreateMyMeeting()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ListName = await _applicationDbContext.ApplicationUser.Where(x=>x.Id != userId).ToListAsync();
            var UserList = new List<SelectListItem>();
            foreach (var item in ListName)
            {
                var itm = new SelectListItem();
                itm.Value = item.Id.ToString();
                itm.Text = item.UserName;
                UserList.Add(itm);
            }
            ViewBag.PackOfUser= UserList;
            var List = await _applicationDbContext.Rooms.ToListAsync();
            var RoomList = new List<SelectListItem>();
            foreach(var item in List)
            {
                var itm = new SelectListItem();
                itm.Value = item.Id.ToString();
                itm.Text = item.RoomName;
                RoomList.Add(itm);
            }
            ViewBag.PackOfRoom = RoomList;
            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> CreateMyMeeting(RoomBooking meeting) 
        {
            if (!ModelState.IsValid)
            {
                await _applicationDbContext.RoomBookings.AddAsync(meeting);
                _applicationDbContext.SaveChanges();
                foreach (var id in meeting.SelectedUser)
                {
                    
                    await _applicationDbContext.Participants.AddAsync(new Participant
                    {
                        RoomBookingId = meeting.Id,
                        UserId = id,
                    }) ;
                }
                _applicationDbContext.SaveChanges();
                return RedirectToAction("ListMyMeeting");
            }
            return View();
            
            
        }
        [HttpGet]
        public async Task<IActionResult> EditMyMeeting(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ListName = await _applicationDbContext.ApplicationUser.Where(x => x.Id != userId).ToListAsync();
            var UserList = new List<SelectListItem>();
            foreach (var item in ListName)
            {
                var itm = new SelectListItem();
                itm.Value = item.Id.ToString();
                itm.Text = item.UserName;
                UserList.Add(itm);
            }
            ViewBag.PackOfUser = UserList;
            var List = await _applicationDbContext.Rooms.ToListAsync();
            var RoomList = new List<SelectListItem>();
            foreach (var item in List)
            {
                var itm = new SelectListItem();
                itm.Value = item.Id.ToString();
                itm.Text = item.RoomName;
                RoomList.Add(itm);
            }
            ViewBag.PackOfRoom = RoomList;

            var meeting = await _applicationDbContext.RoomBookings.FirstOrDefaultAsync(x=>x.Id == id);
            var participant = await (from p in _applicationDbContext.Participants.Where(x => x.RoomBookingId == id) join u in _applicationDbContext.ApplicationUser on p.UserId equals u.Id select p).ToListAsync();
            var listUser = new List<SelectListItem>();
            foreach (var user in participant)
            {
                var itm = new SelectListItem();
                itm.Value = user.ApplicationUser.Id.ToString();
                //itm.Value = user.ApplicationUser.ToString();
                listUser.Add(itm);
            }
            ViewBag.SelectUser = listUser;
            List<string> list = new List<string>();
            foreach (var item in listUser)
            {

                list.Add(item.Value.ToString());
            }
            meeting.SelectedUser = list.ToArray();
            return View(meeting);
        }
        [HttpPost]
        public async Task<ActionResult> EditMyMeeting(RoomBooking model)
        {
            
            if (!ModelState.IsValid)
            {
                _applicationDbContext.RoomBookings.Update(model);
                var user = await _applicationDbContext.Participants.Where(x => x.RoomBookingId == model.Id).ToListAsync();
                _applicationDbContext.Participants.RemoveRange(user);
                _applicationDbContext.SaveChanges();
                foreach (var id in model.SelectedUser)
                {
                    await _applicationDbContext.Participants.AddAsync(new Participant
                    {
                        RoomBookingId = model.Id,
                        UserId = id,
                    });

                }
                _applicationDbContext.SaveChanges();
            }
            else
            {
                return View(model);
            }
            return RedirectToAction("ListMyMeeting");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteMyMeeting(RoomBooking model)
        {
            var meeting = await _applicationDbContext.RoomBookings.FirstOrDefaultAsync(x => x.Id == model.Id);
            var user = await _applicationDbContext.Participants.Where(x => x.RoomBookingId == model.Id).ToListAsync();
            _applicationDbContext.Participants.RemoveRange(user);
            _applicationDbContext.RoomBookings.Remove(meeting);
            
            _applicationDbContext.SaveChanges();
            return RedirectToAction("ListMyMeeting");
        }
        public async Task<IActionResult> ViewMeetingDetail(int id)
        {
            var meeting = await _applicationDbContext.RoomBookings.Include(x => x.RoomStatus).Include(x => x.Room).FirstOrDefaultAsync(x=>x.Id == id);
            return View(meeting);
        }
        
        public IActionResult ViewAllMeeting()
        {
            return View();
        }
        [HttpGet]
        public IActionResult CreateRoom()
        {
            return View();
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
