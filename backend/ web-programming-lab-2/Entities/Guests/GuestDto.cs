namespace web_programming_lab_2.Entities.Guests;


public record GuestDtoGet
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

public record GuestDtoCreate
{
    public string Name { get; set; }
    public string Email { get; set; }
}