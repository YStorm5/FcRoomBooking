using FcRoomBooking.Models.Domain;

namespace FcRoomBooking.Models.ViewModel
{
    public class ParticipantViewModel
    {
        public string[] ParticipantId { get; set; }
        public int RoomBookingId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
}
