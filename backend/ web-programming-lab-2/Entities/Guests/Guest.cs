using web_programming_lab_2.Entities.Reservations;

namespace web_programming_lab_2.Entities.Guests;

public class Guest
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    
    public ICollection<Reservation>? Reservations { get; set; }
}