using System.ComponentModel.DataAnnotations;

namespace FcRoomBooking.Models.Domain
{
    public class RoomStatus
    {
        [Key]
        public int Id { get; set; }
        public string Status { get; set; }
    }
}
