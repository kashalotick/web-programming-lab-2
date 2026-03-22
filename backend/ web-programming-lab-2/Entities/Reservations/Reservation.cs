using web_programming_lab_2.Entities.Guests;
using web_programming_lab_2.Entities.Rooms;

namespace web_programming_lab_2.Entities.Reservations;

public class Reservation
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public int GuestId { get; set; }
    
    public int GuestCount { get; set; }

    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public int GrandTotal { get; set; }
    public bool IsActive { get; set; } = true;
    
    public Room? Room { get; set; }
    public Guest? Guest { get; set; }
}