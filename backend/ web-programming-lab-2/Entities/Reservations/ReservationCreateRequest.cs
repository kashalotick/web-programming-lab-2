using web_programming_lab_2.Entities.Guests;

namespace web_programming_lab_2.Entities.Reservations;

public record ReservationCreateRequest
{
    public ReservationDtoCreate Reservation { get; set; }
    public GuestDtoCreate Guest { get; set; }
}

