namespace web_programming_lab_2.Entities.Reservations;

public class ReservationDtoCreate
{
    public int RoomId { get; set; }
    public int GuestCount { get; set; }
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
}

