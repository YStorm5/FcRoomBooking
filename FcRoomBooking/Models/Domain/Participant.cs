using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace FcRoomBooking.Models.Domain
{
    [Keyless]
    public class Participant
    {
        
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        [ForeignKey("RoomBooking")]
        public int RoomBookingId { get; set; }
        public RoomBooking RoomBooking { get; set; }
    }
}
