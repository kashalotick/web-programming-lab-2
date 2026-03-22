namespace web_programming_lab_2.Entities.Reservations;

public record ReservationDtoGet
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public int GuestId { get; set; }
    
    public int GuestCount { get; set; }

    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public int GrandTotal { get; set; }
    public bool IsActive { get; set; }
}