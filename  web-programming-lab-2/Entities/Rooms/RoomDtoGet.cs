using web_programming_lab_2.Entities.Reservations;

namespace web_programming_lab_2.Entities.Rooms;

public record RoomDtoGet
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Capacity { get; set; }
    public int Price { get; set; }
}