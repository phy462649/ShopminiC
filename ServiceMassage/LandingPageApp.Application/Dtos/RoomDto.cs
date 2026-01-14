namespace LandingPageApp.Application.Dtos;

#region Response DTOs

public class RoomDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? Capacity { get; set; }
    public bool Active { get; set; }
}

#endregion

#region Request DTOs

public class CreateRoomDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Capacity { get; set; } = 1;
    public bool Active { get; set; } = true;
}

public class UpdateRoomDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Capacity { get; set; }
    public bool Active { get; set; } = true;
}

#endregion
