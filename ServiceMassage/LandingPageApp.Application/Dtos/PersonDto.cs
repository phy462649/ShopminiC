namespace LandingPageApp.Application.Dtos;

#region Response DTOs

public class PersonDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string Username { get; set; } = string.Empty;
    public long RoleId { get; set; }
    public string? RoleName { get; set; }
    public bool StatusVerify { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class PersonDetailDto : PersonDto
{
    public int BookingCount { get; set; }
    public int OrderCount { get; set; }
}

#endregion

#region Request DTOs

public class CreatePersonDto
{
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public long RoleId { get; set; }
}

public class UpdatePersonDto
{
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public long RoleId { get; set; }
}

public class ChangePasswordDto
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

#endregion
