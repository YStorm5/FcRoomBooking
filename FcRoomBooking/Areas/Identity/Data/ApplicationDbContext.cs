using FcRoomBooking.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FcRoomBooking.Areas.Identity.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<RoomBooking> RoomBookings { get; set; }
    public DbSet<Participant> Participants { get; set; }
    public DbSet<RoomStatus> RoomStatus { get; set; }
    public DbSet<ApplicationUser> ApplicationUser { get; set; }
    public DbSet<ApplicationRole> ApplicationRole { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Participant>()
            .HasKey(t => new { t.UserId, t.RoomBookingId });
        builder.Entity<Participant>()
            .HasOne(pt => pt.ApplicationUser)
            .WithMany(p => p.Participant)
            .HasForeignKey(p => p.UserId);
        builder.Entity<Participant>()
            .HasOne(pt => pt.RoomBooking)
            .WithMany(p => p.Participant)
            .HasForeignKey(p => p.RoomBookingId);

        base.OnModelCreating(builder);
        
    }
}
