namespace web_programming_lab_2.Entities.Reservations;

public record ReservationDtoCreate
{
    public int RoomId { get; set; }

    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
}