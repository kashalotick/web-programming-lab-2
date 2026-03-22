namespace web_programming_lab_2.Entities.Reservations;

public class ReservationFilter
{
    public int? RoomId { get; set; }
    public bool? IsActive { get; set; }

    public string? SortBy { get; set; }
    public bool IsDescending { get; set; } = false;
    
}