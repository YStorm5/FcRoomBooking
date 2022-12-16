using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FcRoomBooking.Models.Domain
{
    public class Room
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? RoomName { get; set; } 

        public string? RoomCapacity { get; set; }
        [Required]
        public string? RoomDescription { get; set; }
        [Required]
        public byte[]? RoomImage { get; set; }
        [ForeignKey("RoomStatus")]
        public int? RoomStatusId { get; set; }
        public RoomStatus? RoomStatus { get; set; }
        [NotMapped]
        public IFormFile? image { get; set; }

        [InverseProperty(nameof(FcRoomBooking.Models.Domain.RoomBooking.Room))]
        public virtual ICollection<RoomBooking> RoomBooking { get; set; }

    }
}
