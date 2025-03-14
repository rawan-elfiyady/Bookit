using System;

namespace Bookit.Models;

public class UpdateUserDto
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public bool IsApproved { get; set; }
}
