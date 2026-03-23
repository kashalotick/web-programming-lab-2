using web_programming_lab_2.Entities.Guests;
using web_programming_lab_2.Entities.Rooms;

namespace web_programming_lab_2.Entities.Reservations;

public record ReservationCreateRequest
{
    public ReservationDtoCreate Reservation { get; set; }
    public GuestDtoCreate Guest { get; set; }
}

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

public record ReservationDtoFullGet
{
    public int Id { get; set; }
    public RoomDtoGet Room { get; set; }
    public GuestDtoGet Guest { get; set; }

    public int GuestCount { get; set; }

    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    public DateTime CreatedAt { get; set; }

    public int GrandTotal { get; set; }
    public bool IsActive { get; set; }
}

public record ReservationDtoCreate
{
    public int RoomId { get; set; }
    public int GuestCount { get; set; }
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    public int? GrandTotal { get; set; } // admin only
}

public record ReservationDtoUpdate
{
    public int? RoomId { get; set; }
    public int? GuestCount { get; set; }
    public DateOnly? CheckIn { get; set; }
    public DateOnly? CheckOut { get; set; }
    public int? GrandTotal { get; set; }
}